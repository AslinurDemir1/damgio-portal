
using Damgio.Core.Constants;
using Damgio.Core.Enums;
using Damgio.Infrastructure.Data;
using Damgio.Infrastructure.Identity;
using Damgio.Web.Models.Desk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Damgio.Web.Controllers;

[Authorize(Roles = Roles.Cashier + "," + Roles.Admin)]
public class DeskController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public DeskController(
        AppDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Desk
    public async Task<IActionResult> Index(int? shopId, string? query)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
            return Challenge();

        var isAdmin = User.IsInRole(Roles.Admin);

        // Kullanıcının erişebileceği işletmeleri belirle.
        var shopsQuery = _context.Shops
            .AsNoTracking()
            .Where(s => s.IsActive);

        if (!isAdmin)
        {
            var memberShopIds = _context.ShopMembers
                .Where(m => m.UserId == userId)
                .Select(m => m.ShopId);

            shopsQuery = shopsQuery
                .Where(s => memberShopIds.Contains(s.Id));
        }

        var shops = await shopsQuery
            .OrderBy(s => s.Name)
            .Select(s => new DeskShopViewModel
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();

        var model = new DeskIndexViewModel
        {
            SelectedShopId = shopId,
            SearchQuery = query?.Trim(),
            Shops = shops
        };

        // İşletme seçilmediyse veya arama yapılmadıysa
        // müşteri kartlarını listeleme.
        if (!shopId.HasValue ||
            string.IsNullOrWhiteSpace(model.SearchQuery))
        {
            return View(model);
        }

        // Gönderilen işletme kullanıcının yetkisinde mi?
        if (!shops.Any(s => s.Id == shopId.Value))
            return Forbid();

        var searchText = model.SearchQuery;
        var pattern = $"%{searchText}%";

        // Kartları müşteri hesaplarıyla eşleştir.
        var cardsQuery = _context.StampCards
            .AsNoTracking()
            .Where(c => c.Program != null &&
                        c.Program.ShopId == shopId.Value)
            .Join(
                _userManager.Users.AsNoTracking(),
                card => card.UserId,
                user => user.Id,
                (card, user) => new { Card = card, User = user });

        // Kart numarası, e-posta veya telefon ile ara.
        var isCardId = int.TryParse(searchText, out var cardId);

        cardsQuery = cardsQuery.Where(x =>
            (isCardId && x.Card.Id == cardId) ||
            (x.User.Email != null &&
             EF.Functions.ILike(x.User.Email, pattern)) ||
            (x.User.PhoneNumber != null &&
             EF.Functions.ILike(x.User.PhoneNumber, pattern)));

        var results = await cardsQuery
            .OrderBy(x => x.User.FullName)
            .Take(50)
            .Select(x => new DeskCardViewModel
            {
                CardId = x.Card.Id,
                CustomerName = x.User.FullName,
                Email = x.User.Email,
                PhoneNumber = x.User.PhoneNumber,
                ShopName = x.Card.Program!.Shop!.Name,
                ProgramName = x.Card.Program.Name,
                RewardName = x.Card.Program.RewardName,
                CurrentStamps = x.Card.CurrentStamps,
                RequiredStamps = x.Card.Program.RequiredStamps,
                LifetimeStamps = x.Card.LifetimeStamps
            })
            .ToListAsync();

        // Kazanılmış ve kullanılmış ödülleri hesapla.
        var cardIds = results.Select(c => c.CardId).ToList();

        var rewardStats = await _context.StampEvents
            .AsNoTracking()
            .Where(e => cardIds.Contains(e.StampCardId))
            .GroupBy(e => e.StampCardId)
            .Select(g => new
            {
                CardId = g.Key,
                Earned = g.Count(e => e.Type == StampEventType.RewardEarned),
                Used = g.Count(e => e.Type == StampEventType.RewardUsed)
            })
            .ToDictionaryAsync(x => x.CardId);

        foreach (var card in results)
        {
            if (rewardStats.TryGetValue(card.CardId, out var stats))
            {
                card.AvailableRewards = Math.Max(
                    0,
                    stats.Earned - stats.Used);
            }
        }

        model.Cards = results;

        return View(model);
    }
}
