using Damgio.Core.Common;
using Damgio.Core.Entities;
using Damgio.Core.Enums;
using Damgio.Infrastructure.Data;
using Damgio.Infrastructure.Identity;
using Damgio.Web.Models.Api.Common;
using Damgio.Web.Models.Api.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Damgio.Web.Controllers.Api;

/// <summary>
/// Müşteri mobil işlemleri API'si.
/// iOS istemcisi (damgio-ios) ve mobil tarayıcı için kart sorgulama ve NFC dokunma işlemlerini sağlar.
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class CustomerController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CustomerController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Oturum açmış müşterinin sahip olduğu tüm damga kartlarını listeler.
    /// iOS istemcisinde 'Cüzdanım / Kartlarım' ekranını besler.
    /// </summary>
    /// <returns>Müşterinin damga kartları listesi</returns>
    /// <response code="200">Kartlar başarıyla getirildi</response>
    /// <response code="401">Kullanıcı kimliği doğrulanmamış</response>
    [HttpGet("me/cards")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<CustomerCardSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CustomerCardSummaryDto>>> GetMyCards()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiErrorResponse("UNAUTHENTICATED", "Geçerli bir müşteri oturumu bulunamadı."));
        }

        var cards = await _context.StampCards
            .Include(c => c.Program!)
                .ThenInclude(p => p.Shop!)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var result = cards.Select(c =>
        {
            var cooldown = c.Program?.StampCooldownSeconds ?? 0;
            var canStamp = true;
            if (c.LastStampAt.HasValue && cooldown > 0)
            {
                canStamp = (now - c.LastStampAt.Value).TotalSeconds >= cooldown;
            }

            return new CustomerCardSummaryDto
            {
                CardId = c.Id,
                ShopId = c.Program?.ShopId ?? 0,
                ShopName = c.Program?.Shop?.Name ?? string.Empty,
                ShopSlug = c.Program?.Shop?.Slug ?? string.Empty,
                ProgramId = c.ProgramId,
                ProgramName = c.Program?.Name ?? string.Empty,
                RewardName = c.Program?.RewardName ?? string.Empty,
                CurrentStamps = c.CurrentStamps,
                RequiredStamps = c.Program?.RequiredStamps ?? 7,
                LifetimeStamps = c.LifetimeStamps,
                LastStampAt = c.LastStampAt,
                CanStampNow = canStamp,
                CooldownSeconds = cooldown
            };
        });

        return Ok(result);
    }

    /// <summary>
    /// NFC sayaç tokmak dokunma uç noktası (iOS ve API istemcileri için).
    /// Karar D09: Tokmaktan okunan 'key' doğrulanır.
    /// Karar D07: Soğuma süresi kontrol edilir.
    /// Karar D08: Hedef damgaya ulaşıldığında ödül hak edilir ve döngü sıfırlanır.
    /// </summary>
    /// <param name="request">İşletme slug ve cihaz gizli anahtarı</param>
    /// <returns>İşlem sonucu ve güncel kart durumu</returns>
    /// <response code="200">Damga veya ödül başarıyla işlendi</response>
    /// <response code="400">Doğrulama veya cihaz hatası</response>
    /// <response code="401">Kullanıcı kimliği doğrulanmamış</response>
    /// <response code="429">Soğuma süresi aktif (Cooldown)</response>
    [HttpPost("tap")]
    [Authorize]
    [ProducesResponseType(typeof(TapResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<TapResponseDto>> Tap([FromBody] TapRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponse("VALIDATION", "İstek verileri doğrulamadan geçemedi."));
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiErrorResponse("UNAUTHENTICATED", "Geçerli bir müşteri oturumu bulunamadı."));
        }

        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();
        var shop = await _context.Shops
            .Include(s => s.Programs)
            .FirstOrDefaultAsync(s => s.Slug == normalizedSlug);

        if (shop is null)
        {
            return NotFound(new ApiErrorResponse("NOT_FOUND", $"'{request.Slug}' slug değerine sahip işletme bulunamadı."));
        }

        if (!shop.IsActive)
        {
            return BadRequest(new ApiErrorResponse("SHOP_INACTIVE", "İşletme şu anda aktif değildir."));
        }

        // Cihaz anahtarı SHA-256 doğrulaması (D09)
        var keyHash = DeviceKeyHelper.ComputeHash(request.Key);
        var device = await _context.Devices
            .Include(d => d.Program)
            .FirstOrDefaultAsync(d => d.ShopId == shop.Id && d.KeyHash == keyHash && d.IsActive);

        if (device is null)
        {
            return BadRequest(new ApiErrorResponse("INVALID_DEVICE", "Geçersiz veya yetkilendirilmemiş NFC sayaç tokmak anahtarı."));
        }

        var program = device.Program
                      ?? shop.Programs.FirstOrDefault(p => p.Id == device.ProgramId)
                      ?? shop.Programs.FirstOrDefault(p => p.IsActive);

        if (program is null || !program.IsActive)
        {
            return BadRequest(new ApiErrorResponse("VALIDATION", "Cihaza bağlı sadakat programı aktif değildir."));
        }

        // Müşteri kartını al veya oluştur (otomatik kayıt)
        var card = await _context.StampCards
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProgramId == program.Id);

        var now = DateTime.UtcNow;

        if (card is null)
        {
            card = new StampCard
            {
                UserId = userId,
                ProgramId = program.Id,
                CurrentStamps = 0,
                LifetimeStamps = 0,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.StampCards.Add(card);
            await _context.SaveChangesAsync();
        }

        // Soğuma süresi kontrolü (D07)
        if (card.LastStampAt.HasValue && program.StampCooldownSeconds > 0)
        {
            var elapsed = (now - card.LastStampAt.Value).TotalSeconds;
            if (elapsed < program.StampCooldownSeconds)
            {
                var remaining = (int)Math.Ceiling(program.StampCooldownSeconds - elapsed);
                return StatusCode(StatusCodes.Status429TooManyRequests,
                    new ApiErrorResponse("COOLDOWN", $"Soğuma süresi aktif. Lütfen {remaining} saniye bekleyin."));
            }
        }

        // Damga ve Ödül Döngüsü Mantığı (D08)
        var nextStamps = card.CurrentStamps + 1;
        card.LifetimeStamps += 1;
        var rewardEarned = false;
        string message;

        if (nextStamps >= program.RequiredStamps)
        {
            // Ödül hak edildi: Döngü sıfırlanır
            rewardEarned = true;
            card.CurrentStamps = 0;

            _context.StampEvents.Add(new StampEvent
            {
                StampCardId = card.Id,
                Type = StampEventType.RewardEarned,
                Source = StampSource.Tap,
                DeviceId = device.Id,
                CreatedAt = now
            });

            message = $"Tebrikler! {program.RewardName} ödülünüzü kazandınız! Damgalarınız sıfırlandı ve yeni döngünüz başladı.";
        }
        else
        {
            // Standart damga
            card.CurrentStamps = nextStamps;

            _context.StampEvents.Add(new StampEvent
            {
                StampCardId = card.Id,
                Type = StampEventType.Stamp,
                Source = StampSource.Tap,
                DeviceId = device.Id,
                CreatedAt = now
            });

            message = $"1 damga eklendi ({card.CurrentStamps}/{program.RequiredStamps}).";
        }

        card.LastStampAt = now;
        card.UpdatedAt = now;

        await _context.SaveChangesAsync();

        var response = new TapResponseDto
        {
            Success = true,
            Message = message,
            RewardEarned = rewardEarned,
            CurrentStamps = card.CurrentStamps,
            RequiredStamps = program.RequiredStamps,
            LifetimeStamps = card.LifetimeStamps,
            RewardName = program.RewardName,
            CardId = card.Id,
            ShopName = shop.Name,
            RemainingCooldownSeconds = program.StampCooldownSeconds
        };

        return Ok(response);
    }
}
