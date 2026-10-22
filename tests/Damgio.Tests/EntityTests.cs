using Damgio.Core.Constants;
using Damgio.Core.Entities;
using Damgio.Core.Enums;
using Xunit;

namespace Damgio.Tests;

public class EntityTests
{
    [Fact]
    public void Roles_Contains_ThreeRequiredRoles()
    {
        Assert.Contains(Roles.Admin, Roles.All);
        Assert.Contains(Roles.Cashier, Roles.All);
        Assert.Contains(Roles.Customer, Roles.All);
        Assert.Equal(3, Roles.All.Length);
    }

    [Fact]
    public void LoyaltyProgram_DefaultValues_AreConsistent()
    {
        var program = new LoyaltyProgram
        {
            Name = "Kahve Kartı",
            RewardName = "1 Ücretsiz Filtre Kahve"
        };

        Assert.Equal(7, program.RequiredStamps);
        Assert.Equal(180, program.StampCooldownSeconds);
        Assert.True(program.IsActive);
    }

    [Fact]
    public void Shop_InitializesWithEmptyCollections()
    {
        var shop = new Shop
        {
            Name = "Kadıköy Damgio Cafe",
            Slug = "kadikoy-cafe"
        };

        Assert.NotNull(shop.Programs);
        Assert.NotNull(shop.Members);
        Assert.NotNull(shop.Devices);
        Assert.True(shop.IsActive);
    }

    [Fact]
    public void StampCard_TracksStampsCorrectly()
    {
        var card = new StampCard
        {
            UserId = "user-123",
            ProgramId = 1,
            CurrentStamps = 3,
            LifetimeStamps = 10
        };

        Assert.Equal(3, card.CurrentStamps);
        Assert.Equal(10, card.LifetimeStamps);
    }
}
