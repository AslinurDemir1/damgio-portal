using Damgio.Core.Common;
using Damgio.Core.Constants;
using Damgio.Core.Entities;
using Damgio.Infrastructure.Data;
using Damgio.Infrastructure.Identity;
using Damgio.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Damgio.Web.Controllers;

[Authorize(Roles = Roles.Admin)]
public class ShopsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public ShopsController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Shops
    public async Task<IActionResult> Index()
    {
        var shops = await _context.Shops
            .Include(s => s.Programs)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return View(shops);
    }

    // GET: /Shops/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var shop = await _context.Shops
            .Include(s => s.Programs)
            .Include(s => s.Members)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (shop == null)
            return NotFound();

        var memberUserIds = shop.Members.Select(m => m.UserId).Distinct().ToList();
        var users = await _userManager.Users
            .Where(u => memberUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => new { u.FullName, u.Email });

        var memberViewModels = shop.Members
            .Select(m =>
            {
                users.TryGetValue(m.UserId, out var u);
                return new ShopMemberViewModel
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    FullName = u?.FullName,
                    Email = u?.Email,
                    Role = m.Role,
                    CreatedAt = m.CreatedAt
                };
            })
            .OrderBy(m => m.Role)
            .ThenBy(m => m.FullName)
            .ToList();

        var viewModel = new ShopDetailsViewModel
        {
            Shop = shop,
            Members = memberViewModels,
            NewMember = new ShopMemberInputModel { ShopId = shop.Id }
        };

        return View(viewModel);
    }

    // POST: /Shops/AddMember
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMember(ShopMemberInputModel model)
    {
        var shop = await _context.Shops.FindAsync(model.ShopId);
        if (shop == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Lütfen geçerli bir e-posta adresi ve rol giriniz.";
            return RedirectToAction(nameof(Details), new { id = model.ShopId });
        }

        var user = await _userManager.FindByEmailAsync(model.UserEmail.Trim());
        if (user == null)
        {
            TempData["ErrorMessage"] = $"'{model.UserEmail}' adresine sahip kullanıcı sistemde bulunamadı. Kullanıcının önce kayıt olması gerekir.";
            return RedirectToAction(nameof(Details), new { id = model.ShopId });
        }

        var alreadyMember = await _context.ShopMembers
            .AnyAsync(m => m.ShopId == model.ShopId && m.UserId == user.Id);

        if (alreadyMember)
        {
            TempData["ErrorMessage"] = $"'{user.FullName ?? user.Email}' kullanıcısı bu işletmeye zaten atanmış.";
            return RedirectToAction(nameof(Details), new { id = model.ShopId });
        }

        var member = new ShopMember
        {
            ShopId = model.ShopId,
            UserId = user.Id,
            Role = model.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.ShopMembers.Add(member);

        // Kasiyer rolü atandıysa ve kullanıcıda Cashier rolü yoksa Identity rolünü de ekle
        if (model.Role == Core.Enums.ShopRole.Cashier && !await _userManager.IsInRoleAsync(user, Roles.Cashier))
        {
            await _userManager.AddToRoleAsync(user, Roles.Cashier);
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{user.FullName ?? user.Email}' başarıyla {model.Role} olarak işletmeye eklendi.";
        return RedirectToAction(nameof(Details), new { id = model.ShopId });
    }

    // POST: /Shops/RemoveMember
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMember(int shopId, int memberId)
    {
        var member = await _context.ShopMembers
            .FirstOrDefaultAsync(m => m.Id == memberId && m.ShopId == shopId);

        if (member != null)
        {
            _context.ShopMembers.Remove(member);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Personel işletmeden çıkarıldı.";
        }

        return RedirectToAction(nameof(Details), new { id = shopId });
    }

    // GET: /Shops/Create
    public IActionResult Create()
    {
        return View(new ShopInputModel());
    }

    // POST: /Shops/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ShopInputModel model)
    {
        // 1. Slug otomatik üretimi veya temizlenmesi
        var slug = string.IsNullOrWhiteSpace(model.Slug)
            ? SlugHelper.ToSlug(model.Name)
            : SlugHelper.ToSlug(model.Slug);

        model.Slug = slug;

        // 2. Slug kuralları denetimi
        if (string.IsNullOrWhiteSpace(slug))
        {
            ModelState.AddModelError(nameof(model.Slug), "İşletme adı geçerli bir bağlantı adresi (slug) üretilebilecek karakterler içermelidir.");
        }
        else if (SlugHelper.IsReservedSlug(slug))
        {
            ModelState.AddModelError(nameof(model.Slug), $"'{slug}' sistem tarafından rezerve edilmiş bir adres olduğundan kullanılamaz.");
        }
        else if (!SlugHelper.IsValidSlug(slug))
        {
            ModelState.AddModelError(nameof(model.Slug), "Bağlantı adresi yalnızca küçük harf, rakam ve tek tire içerebilir (Örn: kadikoy-cafe).");
        }
        else if (await _context.Shops.AnyAsync(s => s.Slug == slug))
        {
            ModelState.AddModelError(nameof(model.Slug), $"'{slug}' bağlantı adresi başka bir işletme tarafından kullanılmaktadır.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var shop = new Shop
        {
            Name = model.Name.Trim(),
            Slug = slug,
            Address = model.Address?.Trim(),
            IsActive = model.IsActive
        };

        _context.Shops.Add(shop);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{shop.Name}' işletmesi başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Shops/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var shop = await _context.Shops.FindAsync(id);
        if (shop == null)
            return NotFound();

        var model = new ShopInputModel
        {
            Id = shop.Id,
            Name = shop.Name,
            Slug = shop.Slug,
            Address = shop.Address,
            IsActive = shop.IsActive
        };

        return View(model);
    }

    // POST: /Shops/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ShopInputModel model)
    {
        if (id != model.Id)
            return NotFound();

        var slug = string.IsNullOrWhiteSpace(model.Slug)
            ? SlugHelper.ToSlug(model.Name)
            : SlugHelper.ToSlug(model.Slug);

        model.Slug = slug;

        if (string.IsNullOrWhiteSpace(slug))
        {
            ModelState.AddModelError(nameof(model.Slug), "Geçerli bir bağlantı adresi (slug) giriniz.");
        }
        else if (SlugHelper.IsReservedSlug(slug))
        {
            ModelState.AddModelError(nameof(model.Slug), $"'{slug}' sistem tarafından rezerve edilmiş bir adres olduğundan kullanılamaz.");
        }
        else if (!SlugHelper.IsValidSlug(slug))
        {
            ModelState.AddModelError(nameof(model.Slug), "Bağlantı adresi formatı geçersiz.");
        }
        else if (await _context.Shops.AnyAsync(s => s.Slug == slug && s.Id != id))
        {
            ModelState.AddModelError(nameof(model.Slug), $"'{slug}' bağlantı adresi başka bir işletme tarafından kullanılmaktadır.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var shop = await _context.Shops.FindAsync(id);
        if (shop == null)
            return NotFound();

        shop.Name = model.Name.Trim();
        shop.Slug = slug;
        shop.Address = model.Address?.Trim();
        shop.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{shop.Name}' işletme bilgileri güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Shops/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var shop = await _context.Shops
            .Include(s => s.Programs)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (shop == null)
            return NotFound();

        return View(shop);
    }

    // POST: /Shops/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var shop = await _context.Shops.FindAsync(id);
        if (shop != null)
        {
            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"'{shop.Name}' işletmesi silindi.";
        }

        return RedirectToAction(nameof(Index));
    }
}
