using Damgio.Core.Common;
using Damgio.Core.Constants;
using Damgio.Core.Entities;
using Damgio.Core.Enums;
using Damgio.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Damgio.Infrastructure.Data;

public static class DbSeeder
{
    public const string DefaultDeviceKey = "damgio-tokmak-secret-2026";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 1. Rollerin oluşturulması
        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"'{roleName}' rolü oluşturulamadı: {errors}");
                }
            }
        }

        // 2. Varsayılan Admin Kullanıcısı
        var adminEmail = "admin@damgio.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Sistem Yöneticisi",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123*");
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Varsayılan yönetici oluşturulamadı: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }

        // 3. Varsayılan Kasiyer Kullanıcısı
        var cashierEmail = "kasiyer@damgio.com";
        var cashierUser = await userManager.FindByEmailAsync(cashierEmail);
        if (cashierUser is null)
        {
            cashierUser = new AppUser
            {
                UserName = cashierEmail,
                Email = cashierEmail,
                FullName = "Demo Kasiyer",
                EmailConfirmed = true,
                PhoneNumber = "05551112233"
            };

            var result = await userManager.CreateAsync(cashierUser, "Kasiyer123*");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(cashierUser, Roles.Cashier);
            }
        }

        // 4. Varsayılan Müşteri Kullanıcısı
        var customerEmail = "musteri@damgio.com";
        var customerUser = await userManager.FindByEmailAsync(customerEmail);
        if (customerUser is null)
        {
            customerUser = new AppUser
            {
                UserName = customerEmail,
                Email = customerEmail,
                FullName = "Demo Müşteri",
                EmailConfirmed = true,
                PhoneNumber = "05559998877"
            };

            var result = await userManager.CreateAsync(customerUser, "Musteri123*");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customerUser, Roles.Customer);
            }
        }

        // 5. Örnek İşletme, Program ve NFC Cihazı
        var demoShop = await dbContext.Shops.Include(s => s.Programs).FirstOrDefaultAsync(s => s.Slug == "kadikoy-damgio-cafe");
        if (demoShop is null)
        {
            var now = DateTime.UtcNow;
            demoShop = new Shop
            {
                Name = "Kadıköy Damgio Cafe",
                Slug = "kadikoy-damgio-cafe",
                Address = "Moda Caddesi No: 42, Kadıköy, İstanbul",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            var program = new LoyaltyProgram
            {
                Name = "7 Kahveye 1 İkram",
                RequiredStamps = 7,
                RewardName = "1 Adet Filtre Kahve",
                StampCooldownSeconds = 180,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            demoShop.Programs.Add(program);
            dbContext.Shops.Add(demoShop);
            await dbContext.SaveChangesAsync();

            // NFC Cihazı (Tokmak) Tanımlama
            var deviceKeyHash = DeviceKeyHelper.ComputeHash(DefaultDeviceKey);
            var device = new Device
            {
                ShopId = demoShop.Id,
                ProgramId = program.Id,
                Name = "Kasa 1 NFC Tokmak",
                KeyHash = deviceKeyHash,
                IsActive = true,
                CreatedAt = now
            };

            dbContext.Devices.Add(device);

            // Kasiyeri işletmeye bağlama
            if (cashierUser != null)
            {
                dbContext.ShopMembers.Add(new ShopMember
                {
                    ShopId = demoShop.Id,
                    UserId = cashierUser.Id,
                    Role = ShopRole.Cashier,
                    CreatedAt = now
                });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
