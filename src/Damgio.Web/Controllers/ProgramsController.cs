using Damgio.Core.Constants;
using Damgio.Core.Entities;
using Damgio.Infrastructure.Data;
using Damgio.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Damgio.Web.Controllers;

[Authorize(Roles = Roles.Admin)]
public class ProgramsController : Controller
{
    private readonly AppDbContext _context;

    public ProgramsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Programs
    public async Task<IActionResult> Index(int? shopId)
    {
        var query = _context.Programs
            .Include(p => p.Shop)
            .Include(p => p.StampCards)
            .AsQueryable();

        if (shopId.HasValue)
        {
            query = query.Where(p => p.ShopId == shopId.Value);
            ViewData["CurrentShop"] = await _context.Shops.FindAsync(shopId.Value);
        }

        var programs = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        ViewData["ShopId"] = shopId;
        return View(programs);
    }

    // GET: /Programs/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var program = await _context.Programs
            .Include(p => p.Shop)
            .Include(p => p.StampCards)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (program == null)
            return NotFound();

        return View(program);
    }

    // GET: /Programs/Create
    public async Task<IActionResult> Create(int? shopId)
    {
        await PopulateShopsDropDownListAsync(shopId);
        var model = new ProgramInputModel
        {
            ShopId = shopId ?? 0,
            RequiredStamps = 7,
            RewardName = "1 Ücretsiz İçecek",
            StampCooldownSeconds = 180,
            IsActive = true
        };

        return View(model);
    }

    // POST: /Programs/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProgramInputModel model)
    {
        // İşletme varlığı kontrolü
        var shopExists = await _context.Shops.AnyAsync(s => s.Id == model.ShopId);
        if (!shopExists)
        {
            ModelState.AddModelError(nameof(model.ShopId), "Seçilen işletme sistemde bulunamadı.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateShopsDropDownListAsync(model.ShopId);
            return View(model);
        }

        var program = new LoyaltyProgram
        {
            ShopId = model.ShopId,
            Name = model.Name.Trim(),
            RequiredStamps = model.RequiredStamps,
            RewardName = model.RewardName.Trim(),
            StampCooldownSeconds = model.StampCooldownSeconds,
            IsActive = model.IsActive
        };

        _context.Programs.Add(program);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{program.Name}' damga programı başarıyla eklendi.";
        return RedirectToAction(nameof(Index), new { shopId = program.ShopId });
    }

    // GET: /Programs/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var program = await _context.Programs.FindAsync(id);
        if (program == null)
            return NotFound();

        await PopulateShopsDropDownListAsync(program.ShopId);

        var model = new ProgramInputModel
        {
            Id = program.Id,
            ShopId = program.ShopId,
            Name = program.Name,
            RequiredStamps = program.RequiredStamps,
            RewardName = program.RewardName,
            StampCooldownSeconds = program.StampCooldownSeconds,
            IsActive = program.IsActive
        };

        return View(model);
    }

    // POST: /Programs/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProgramInputModel model)
    {
        if (id != model.Id)
            return NotFound();

        var shopExists = await _context.Shops.AnyAsync(s => s.Id == model.ShopId);
        if (!shopExists)
        {
            ModelState.AddModelError(nameof(model.ShopId), "Seçilen işletme sistemde bulunamadı.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateShopsDropDownListAsync(model.ShopId);
            return View(model);
        }

        var program = await _context.Programs.FindAsync(id);
        if (program == null)
            return NotFound();

        program.ShopId = model.ShopId;
        program.Name = model.Name.Trim();
        program.RequiredStamps = model.RequiredStamps;
        program.RewardName = model.RewardName.Trim();
        program.StampCooldownSeconds = model.StampCooldownSeconds;
        program.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{program.Name}' programı başarıyla güncellendi.";
        return RedirectToAction(nameof(Index), new { shopId = program.ShopId });
    }

    // GET: /Programs/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var program = await _context.Programs
            .Include(p => p.Shop)
            .Include(p => p.StampCards)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (program == null)
            return NotFound();

        return View(program);
    }

    // POST: /Programs/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var program = await _context.Programs.FindAsync(id);
        if (program != null)
        {
            var shopId = program.ShopId;
            _context.Programs.Remove(program);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"'{program.Name}' programı silindi.";
            return RedirectToAction(nameof(Index), new { shopId });
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateShopsDropDownListAsync(object? selectedShop = null)
    {
        var shops = await _context.Shops
            .OrderBy(s => s.Name)
            .ToListAsync();

        ViewBag.ShopList = new SelectList(shops, "Id", "Name", selectedShop);
    }
}
