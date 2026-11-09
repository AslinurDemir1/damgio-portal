using Damgio.Core.Common;
using Damgio.Core.Entities;
using Damgio.Core.Enums;
using Damgio.Infrastructure.Data;
using Damgio.Infrastructure.Identity;
using Damgio.Web.Models.Customer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Damgio.Web.Controllers;

/// <summary>
/// Müşteri mobil damga kartı arayüzü ve NFC tokmak dokunma akışını yönetir.
/// Karar D09 ve D07: /s/{slug} mobil kart görünümü ve /s/{slug}/tap?k=... NFC tetiklemesi.
/// </summary>
[Route("s/{slug}")]
public class CustomerCardController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CustomerCardController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Müşterinin tarayıcıda açtığı mobil uyumlu damga kartı ekranı.
    /// URL: /s/{slug}
    /// </summary>
    [HttpGet("")]
    public async Task<IActionResult> Index(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var normalizedSlug = slug.Trim().ToLowerInvariant();
        var shop = await _context.Shops
            .Include(s => s.Programs)
            .FirstOrDefaultAsync(s => s.Slug == normalizedSlug);

        if (shop is null)
        {
            ViewBag.Slug = normalizedSlug;
            return View("ShopNotFound");
        }

        if (!shop.IsActive)
        {
            return View("ShopInactive", shop);
        }

        var program = shop.Programs.FirstOrDefault(p => p.IsActive)
                      ?? shop.Programs.FirstOrDefault();

        if (program is null)
        {
            return View("NoActiveProgram", shop);
        }

        var isAuthenticated = User.Identity?.IsAuthenticated == true;
        AppUser? currentUser = null;
        StampCard? card = null;

        if (isAuthenticated)
        {
            currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                // Müşterinin bu programda bir kartı var mı?
                card = await _context.StampCards
                    .Include(c => c.Events)
                    .FirstOrDefaultAsync(c => c.UserId == currentUser.Id && c.ProgramId == program.Id);

                // Kart yoksa otomatik olarak kart oluştur (otomatik kayıt / auto-enrollment)
                if (card is null)
                {
                    var now = DateTime.UtcNow;
                    card = new StampCard
                    {
                        UserId = currentUser.Id,
                        ProgramId = program.Id,
                        CurrentStamps = 0,
                        LifetimeStamps = 0,
                        CreatedAt = now,
                        UpdatedAt = now
                    };

                    _context.StampCards.Add(card);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // Soğuma süresi (Cooldown - D07) kontrolü
        var canStampNow = true;
        var remainingCooldown = 0;
        if (card?.LastStampAt.HasValue == true && program.StampCooldownSeconds > 0)
        {
            var elapsed = (DateTime.UtcNow - card.LastStampAt.Value).TotalSeconds;
            if (elapsed < program.StampCooldownSeconds)
            {
                canStampNow = false;
                remainingCooldown = (int)Math.Ceiling(program.StampCooldownSeconds - elapsed);
            }
        }

        // Son 5 olay geçmişi
        var recentEvents = card?.Events
            .OrderByDescending(e => e.CreatedAt)
            .Take(5)
            .Select(e => new CustomerStampEventItem
            {
                Id = e.Id,
                TypeName = e.Type switch
                {
                    StampEventType.Stamp => "Damga",
                    StampEventType.RewardEarned => "Ödül Kazanıldı",
                    StampEventType.RewardUsed => "Ödül Kullanıldı",
                    _ => e.Type.ToString()
                },
                SourceName = e.Source switch
                {
                    StampSource.Tap => "NFC / QR Dokunma",
                    StampSource.Manual => "Kasa Manuel",
                    _ => e.Source.ToString()
                },
                CreatedAt = e.CreatedAt,
                Description = e.Type switch
                {
                    StampEventType.Stamp => "1 damga eklendi",
                    StampEventType.RewardEarned => $"Tebrikler! {program.RewardName} ödülü hak edildi",
                    StampEventType.RewardUsed => $"{program.RewardName} ödülü kullanıldı",
                    _ => ""
                }
            })
            .ToList() ?? [];

        var viewModel = new CustomerCardViewModel
        {
            CardId = card?.Id,
            ShopName = shop.Name,
            ShopSlug = shop.Slug,
            ShopAddress = shop.Address,
            ProgramName = program.Name,
            ProgramId = program.Id,
            CurrentStamps = card?.CurrentStamps ?? 0,
            RequiredStamps = program.RequiredStamps,
            LifetimeStamps = card?.LifetimeStamps ?? 0,
            RewardName = program.RewardName,
            CooldownSeconds = program.StampCooldownSeconds,
            LastStampAt = card?.LastStampAt,
            IsAuthenticated = isAuthenticated,
            CustomerName = currentUser?.FullName,
            CustomerEmail = currentUser?.Email,
            CanStampNow = canStampNow,
            RemainingCooldownSeconds = remainingCooldown,
            RewardEarnedNotice = TempData["RewardEarned"] as bool? ?? false,
            StatusMessage = TempData["SuccessMessage"] as string,
            WarningMessage = TempData["WarningMessage"] as string,
            ErrorMessage = TempData["ErrorMessage"] as string,
            RecentEvents = recentEvents
        };

        return View(viewModel);
    }

    /// <summary>
    /// NFC sayaç tokmak dokunma uç noktası.
    /// URL: /s/{slug}/tap?k={key}
    /// Karar D09: Tokmak cihazının URL'sinde k parametresi bulunur; veritabanında SHA-256 özeti kontrol edilir.
    /// Karar D07: Cooldown süresi denetlenir.
    /// Karar D08: Hedef damgaya ulaşıldığında ödül hak edilir ve döngü sıfırlanır.
    /// </summary>
    [HttpGet("tap")]
    [HttpPost("tap")]
    public async Task<IActionResult> Tap(string slug, [FromQuery] string? k)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var normalizedSlug = slug.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(k))
        {
            TempData["ErrorMessage"] = "NFC sayaç tokmak anahtarı (k parametresi) eksik veya geçersiz.";
            return RedirectToAction(nameof(Index), new { slug = normalizedSlug });
        }

        // Giriş yapılmamışsa, kullanıcıyı giriş ekranına yönlendirip ardından bu tap URL'sine geri döndürür
        if (User.Identity?.IsAuthenticated != true)
        {
            var returnUrl = $"/s/{normalizedSlug}/tap?k={Uri.EscapeDataString(k)}";
            return RedirectToAction("Login", "Account", new { returnUrl });
        }

        var shop = await _context.Shops
            .Include(s => s.Programs)
            .FirstOrDefaultAsync(s => s.Slug == normalizedSlug);

        if (shop is null)
        {
            TempData["ErrorMessage"] = "İşletme bulunamadı.";
            return RedirectToAction(nameof(Index), new { slug = normalizedSlug });
        }

        if (!shop.IsActive)
        {
            TempData["ErrorMessage"] = "İşletme şu anda aktif değil.";
            return RedirectToAction(nameof(Index), new { slug = normalizedSlug });
        }

        // NFC Cihaz Doğrulaması (Karar D09: KeyHash eşleşmesi)
        var keyHash = DeviceKeyHelper.ComputeHash(k);
        var device = await _context.Devices
            .Include(d => d.Program)
            .FirstOrDefaultAsync(d => d.ShopId == shop.Id && d.KeyHash == keyHash && d.IsActive);

        if (device is null)
        {
            TempData["ErrorMessage"] = "Geçersiz veya yetkilendirilmemiş NFC tokmak anahtarı.";
            return RedirectToAction(nameof(Index), new { slug = normalizedSlug });
        }

        var program = device.Program
                      ?? shop.Programs.FirstOrDefault(p => p.Id == device.ProgramId)
                      ?? shop.Programs.FirstOrDefault(p => p.IsActive);

        if (program is null || !program.IsActive)
        {
            TempData["ErrorMessage"] = "Bu cihaza bağlı sadakat programı aktif değil.";
            return RedirectToAction(nameof(Index), new { slug = normalizedSlug });
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Müşteri kartını getir veya oluştur
        var card = await _context.StampCards
            .FirstOrDefaultAsync(c => c.UserId == currentUser.Id && c.ProgramId == program.Id);

        var now = DateTime.UtcNow;

        if (card is null)
        {
            card = new StampCard
            {
                UserId = currentUser.Id,
                ProgramId = program.Id,
                CurrentStamps = 0,
                LifetimeStamps = 0,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.StampCards.Add(card);
            await _context.SaveChangesAsync();
        }

        // Soğuma süresi kontrolü (Karar D07)
        if (card.LastStampAt.HasValue && program.StampCooldownSeconds > 0)
        {
            var elapsed = (now - card.LastStampAt.Value).TotalSeconds;
            if (elapsed < program.StampCooldownSeconds)
            {
                var waitSeconds = (int)Math.Ceiling(program.StampCooldownSeconds - elapsed);
                TempData["WarningMessage"] = $"Çok hızlı damga basamazsınız! Lütfen {waitSeconds} saniye bekleyin (Soğuma süresi).";
                return RedirectToAction(nameof(Index), new { slug = normalizedSlug });
            }
        }

        // Damga ve Ödül Döngüsü Mantığı (Karar D08)
        var nextStamps = card.CurrentStamps + 1;
        card.LifetimeStamps += 1;

        if (nextStamps >= program.RequiredStamps)
        {
            // Hedefe ulaşıldı: Ödül hak edildi ve damgalar sıfırlandı
            card.CurrentStamps = 0;

            _context.StampEvents.Add(new StampEvent
            {
                StampCardId = card.Id,
                Type = StampEventType.RewardEarned,
                Source = StampSource.Tap,
                DeviceId = device.Id,
                CreatedAt = now
            });

            TempData["RewardEarned"] = true;
            TempData["SuccessMessage"] = $"Tebrikler! {program.RewardName} ödülünüzü kazandınız! Damgalarınız sıfırlandı ve yeni ödül turunuz başladı.";
        }
        else
        {
            // Normal damga eklendi
            card.CurrentStamps = nextStamps;

            _context.StampEvents.Add(new StampEvent
            {
                StampCardId = card.Id,
                Type = StampEventType.Stamp,
                Source = StampSource.Tap,
                DeviceId = device.Id,
                CreatedAt = now
            });

            TempData["SuccessMessage"] = $"Damganız başarıyla basıldı! ({card.CurrentStamps} / {program.RequiredStamps})";
        }

        card.LastStampAt = now;
        card.UpdatedAt = now;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { slug = normalizedSlug });
    }
}
