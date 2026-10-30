using Damgio.Core.Common;
using Damgio.Core.Constants;
using Damgio.Core.Entities;
using Damgio.Infrastructure.Data;
using Damgio.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Damgio.Web.Controllers;

[Authorize(Roles = Roles.Admin)]
public class ShopsController : Controller
{
    private readonly AppDbContext _context;

    public ShopsController(AppDbContext context)
    {
        _context = context;
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
            .FirstOrDefaultAsync(m => m.Id == id);

        if (shop == null)
            return NotFound();

        return View(shop);
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
