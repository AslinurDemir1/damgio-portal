using Damgio.Core.Entities;
using Damgio.Core.Enums;
using Damgio.Infrastructure.Data;
using Damgio.Infrastructure.Identity;
using Damgio.Web.Models.Api.Common;
using Damgio.Web.Models.Api.StampCards;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Damgio.Web.Controllers.Api;

/// <summary>
/// Müşteri damga kartları yönetim API'si (v1).
/// CRUD işlemleri, damga takibi ve ödül döngüsü operasyonlarını sağlar.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class StampCardsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public StampCardsController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Tüm damga kartlarını listeler. İsteğe bağlı olarak kullanıcı, program veya işletmeye göre filtrelenebilir.
    /// </summary>
    /// <param name="userId">Filtrelenecek müşteri kullanıcı ID'si (opsiyonel)</param>
    /// <param name="programId">Filtrelenecek program ID'si (opsiyonel)</param>
    /// <param name="shopId">Filtrelenecek işletme ID'si (opsiyonel)</param>
    /// <returns>Damga kartları listesi</returns>
    /// <response code="200">Kartlar başarıyla listelendi</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StampCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StampCardDto>>> GetAll(
        [FromQuery] string? userId = null,
        [FromQuery] int? programId = null,
        [FromQuery] int? shopId = null)
    {
        var query = _context.StampCards
            .Include(c => c.Program!)
                .ThenInclude(p => p.Shop!)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(c => c.UserId == userId);

        if (programId.HasValue)
            query = query.Where(c => c.ProgramId == programId.Value);

        if (shopId.HasValue)
            query = query.Where(c => c.Program != null && c.Program.ShopId == shopId.Value);

        var cards = await query
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();

        var userIds = cards.Select(c => c.UserId).Distinct().ToList();
        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => new { u.FullName, u.Email });

        var dtos = cards.Select(c =>
        {
            users.TryGetValue(c.UserId, out var userInfo);
            return MapToDto(c, userInfo?.FullName, userInfo?.Email);
        });

        return Ok(dtos);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip damga kartının detaylarını ve geçmiş işlem olaylarını getirir.
    /// </summary>
    /// <param name="id">Damga kartı tekil kimliği</param>
    /// <returns>Damga kartı detayları ve olay geçmişi</returns>
    /// <response code="200">Kart detayları başarıyla getirildi</response>
    /// <response code="404">Belirtilen ID ile kart bulunamadı</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StampCardDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StampCardDetailDto>> GetById(int id)
    {
        var card = await _context.StampCards
            .Include(c => c.Program!)
                .ThenInclude(p => p.Shop!)
            .Include(c => c.Events)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card == null)
        {
            return NotFound(new ApiErrorResponse("NOT_FOUND", $"{id} numaralı damga kartı bulunamadı."));
        }

        var user = await _userManager.FindByIdAsync(card.UserId);

        var detailDto = new StampCardDetailDto
        {
            Id = card.Id,
            UserId = card.UserId,
            UserFullName = user?.FullName,
            UserEmail = user?.Email,
            ProgramId = card.ProgramId,
            ProgramName = card.Program?.Name ?? string.Empty,
            ShopId = card.Program?.ShopId ?? 0,
            ShopName = card.Program?.Shop?.Name ?? string.Empty,
            ShopSlug = card.Program?.Shop?.Slug ?? string.Empty,
            CurrentStamps = card.CurrentStamps,
            RequiredStamps = card.Program?.RequiredStamps ?? 0,
            RewardName = card.Program?.RewardName ?? string.Empty,
            LifetimeStamps = card.LifetimeStamps,
            LastStampAt = card.LastStampAt,
            CreatedAt = card.CreatedAt,
            UpdatedAt = card.UpdatedAt,
            Events = card.Events
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new StampEventDto
                {
                    Id = e.Id,
                    StampCardId = e.StampCardId,
                    Type = e.Type.ToString(),
                    Source = e.Source.ToString(),
                    ActorUserId = e.ActorUserId,
                    DeviceId = e.DeviceId,
                    CreatedAt = e.CreatedAt
                }).ToList()
        };

        return Ok(detailDto);
    }

    /// <summary>
    /// Bir müşteri için belirli bir sadakat programında yeni bir damga kartı oluşturur.
    /// </summary>
    /// <param name="request">Kart oluşturma parametreleri (UserId, ProgramId, InitialStamps)</param>
    /// <returns>Oluşturulan damga kartı bilgisi</returns>
    /// <response code="201">Kart başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz parametreler veya pasif program/işletme</response>
    /// <response code="404">Kullanıcı veya program bulunamadı</response>
    /// <response code="409">Kullanıcı bu programda zaten bir karta sahip</response>
    [HttpPost]
    [ProducesResponseType(typeof(StampCardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StampCardDto>> Create([FromBody] CreateStampCardRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponse("VALIDATION", "İstek verileri doğrulamadan geçemedi."));
        }

        // 1. Kullanıcı kontrolü
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new ApiErrorResponse("NOT_FOUND", $"'{request.UserId}' ID'sine sahip kullanıcı bulunamadı."));
        }

        // 2. Program kontrolü
        var program = await _context.Programs
            .Include(p => p.Shop)
            .FirstOrDefaultAsync(p => p.Id == request.ProgramId);

        if (program == null)
        {
            return NotFound(new ApiErrorResponse("NOT_FOUND", $"{request.ProgramId} numaralı sadakat programı bulunamadı."));
        }

        if (program.Shop != null && !program.Shop.IsActive)
        {
            return BadRequest(new ApiErrorResponse("SHOP_INACTIVE", "İşletme şu anda aktif olmadığı için kart oluşturulamaz."));
        }

        if (!program.IsActive)
        {
            return BadRequest(new ApiErrorResponse("VALIDATION", "Seçilen sadakat programı şu anda aktif değildir."));
        }

        // 3. Tekillik kuralı kontrolü (UserId + ProgramId unique)
        var exists = await _context.StampCards
            .AnyAsync(c => c.UserId == request.UserId && c.ProgramId == request.ProgramId);

        if (exists)
        {
            return Conflict(new ApiErrorResponse("ALREADY_EXISTS", "Bu müşteri için belirtilen programda zaten tanımlı bir damga kartı mevcuttur."));
        }

        var now = DateTime.UtcNow;
        var initialStamps = Math.Max(0, request.InitialStamps);

        var card = new StampCard
        {
            UserId = request.UserId,
            ProgramId = request.ProgramId,
            CurrentStamps = initialStamps,
            LifetimeStamps = initialStamps,
            LastStampAt = initialStamps > 0 ? now : null,
            CreatedAt = now,
            UpdatedAt = now
        };

        if (initialStamps > 0)
        {
            card.Events.Add(new StampEvent
            {
                Type = StampEventType.Stamp,
                Source = StampSource.Manual,
                ActorUserId = _userManager.GetUserId(User),
                CreatedAt = now
            });
        }

        _context.StampCards.Add(card);
        await _context.SaveChangesAsync();

        var dto = MapToDto(card, user.FullName, user.Email, program);

        return CreatedAtAction(nameof(GetById), new { id = card.Id }, dto);
    }

    /// <summary>
    /// Mevcut bir damga kartını günceller (damga sayısı düzeltmesi veya manuel güncelleme).
    /// </summary>
    /// <param name="id">Güncellenecek damga kartı ID'si</param>
    /// <param name="request">Güncellenecek damga değerleri</param>
    /// <returns>Güncellenmiş damga kartı</returns>
    /// <response code="200">Kart başarıyla güncellendi</response>
    /// <response code="400">Geçersiz damga sayısı veya doğrulama hatası</response>
    /// <response code="404">Kart bulunamadı</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(StampCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StampCardDto>> Update(int id, [FromBody] UpdateStampCardRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponse("VALIDATION", "İstek verileri doğrulamadan geçemedi."));
        }

        var card = await _context.StampCards
            .Include(c => c.Program!)
                .ThenInclude(p => p.Shop!)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card == null)
        {
            return NotFound(new ApiErrorResponse("NOT_FOUND", $"{id} numaralı damga kartı bulunamadı."));
        }

        var now = DateTime.UtcNow;
        var requiredStamps = card.Program?.RequiredStamps ?? 7;

        // D08 Ödül Döngüsü Mantığı:
        // Eğer güncellenen damga hedefi aşıyorsa ödül kazanılır ve döngü tamamlanır
        if (request.CurrentStamps >= requiredStamps)
        {
            // Ödül hak edildi olayı
            _context.StampEvents.Add(new StampEvent
            {
                StampCardId = card.Id,
                Type = StampEventType.RewardEarned,
                Source = StampSource.Manual,
                ActorUserId = _userManager.GetUserId(User),
                CreatedAt = now
            });

            card.LifetimeStamps += (request.CurrentStamps - card.CurrentStamps);
            card.CurrentStamps = 0; // Döngü sıfırlanır
        }
        else
        {
            if (request.CurrentStamps > card.CurrentStamps)
            {
                var diff = request.CurrentStamps - card.CurrentStamps;
                card.LifetimeStamps += diff;

                _context.StampEvents.Add(new StampEvent
                {
                    StampCardId = card.Id,
                    Type = StampEventType.Stamp,
                    Source = StampSource.Manual,
                    ActorUserId = _userManager.GetUserId(User),
                    CreatedAt = now
                });
            }

            card.CurrentStamps = request.CurrentStamps;
        }

        if (request.LifetimeStamps.HasValue && request.LifetimeStamps.Value >= card.CurrentStamps)
        {
            card.LifetimeStamps = request.LifetimeStamps.Value;
        }

        card.LastStampAt = now;
        card.UpdatedAt = now;

        await _context.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(card.UserId);
        var dto = MapToDto(card, user?.FullName, user?.Email);

        return Ok(dto);
    }

    /// <summary>
    /// Bir damga kartını ve bağlı işlem geçmişini sistemden siler.
    /// </summary>
    /// <param name="id">Silinecek kart ID'si</param>
    /// <response code="204">Kart başarıyla silindi</response>
    /// <response code="404">Kart bulunamadı</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var card = await _context.StampCards.FindAsync(id);
        if (card == null)
        {
            return NotFound(new ApiErrorResponse("NOT_FOUND", $"{id} numaralı damga kartı bulunamadı."));
        }

        _context.StampCards.Remove(card);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static StampCardDto MapToDto(
        StampCard card,
        string? fullName = null,
        string? email = null,
        LoyaltyProgram? programOverride = null)
    {
        var prog = programOverride ?? card.Program;
        return new StampCardDto
        {
            Id = card.Id,
            UserId = card.UserId,
            UserFullName = fullName,
            UserEmail = email,
            ProgramId = card.ProgramId,
            ProgramName = prog?.Name ?? string.Empty,
            ShopId = prog?.ShopId ?? 0,
            ShopName = prog?.Shop?.Name ?? string.Empty,
            ShopSlug = prog?.Shop?.Slug ?? string.Empty,
            CurrentStamps = card.CurrentStamps,
            RequiredStamps = prog?.RequiredStamps ?? 0,
            RewardName = prog?.RewardName ?? string.Empty,
            LifetimeStamps = card.LifetimeStamps,
            LastStampAt = card.LastStampAt,
            CreatedAt = card.CreatedAt,
            UpdatedAt = card.UpdatedAt
        };
    }
}
