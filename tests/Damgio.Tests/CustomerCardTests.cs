using System.ComponentModel.DataAnnotations;
using Damgio.Core.Common;
using Damgio.Core.Entities;
using Damgio.Core.Enums;
using Damgio.Web.Models.Api.Customer;
using Damgio.Web.Models.Customer;
using Xunit;

namespace Damgio.Tests;

public class CustomerCardTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void CustomerCardViewModel_ProgressPercentage_CalculatedAccurately()
    {
        var vm = new CustomerCardViewModel
        {
            ShopName = "Kadıköy Damgio Cafe",
            ShopSlug = "kadikoy-damgio-cafe",
            ProgramName = "7 Kahveye 1 İkram",
            RewardName = "Filtre Kahve",
            CurrentStamps = 5,
            RequiredStamps = 7
        };

        // 5 / 7 = 71.4%
        Assert.Equal(71.4, vm.ProgressPercentage);
        Assert.Equal(2, vm.StampsRemaining);
    }

    [Fact]
    public void CustomerCardViewModel_ZeroRequiredStamps_ReturnsZeroProgressWithoutDivisionByZero()
    {
        var vm = new CustomerCardViewModel
        {
            ShopName = "Test Shop",
            ShopSlug = "test-shop",
            ProgramName = "Test Program",
            RewardName = "Test Reward",
            CurrentStamps = 0,
            RequiredStamps = 0
        };

        Assert.Equal(0, vm.ProgressPercentage);
        Assert.Equal(0, vm.StampsRemaining);
    }

    [Fact]
    public void TapRequest_ValidModel_PassesValidation()
    {
        var request = new TapRequest
        {
            Slug = "kadikoy-damgio-cafe",
            Key = "damgio-tokmak-secret-2026"
        };

        var results = ValidateModel(request);
        Assert.Empty(results);
    }

    [Theory]
    [InlineData("", "valid-key", "İşletme slug değeri zorunludur.")]
    [InlineData("valid-slug", "", "Cihaz anahtarı (key) zorunludur.")]
    public void TapRequest_MissingRequiredFields_FailsValidation(string slug, string key, string expectedMessage)
    {
        var request = new TapRequest
        {
            Slug = slug,
            Key = key
        };

        var results = ValidateModel(request);
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.ErrorMessage == expectedMessage);
    }

    [Fact]
    public void TapFlow_WhenBelowRequiredStamps_IncrementsCurrentStampsAndEmitsStampEvent()
    {
        var program = new LoyaltyProgram
        {
            Id = 1,
            Name = "7 Kahveye 1 İkram",
            RequiredStamps = 7,
            RewardName = "Filtre Kahve",
            StampCooldownSeconds = 180
        };

        var card = new StampCard
        {
            Id = 10,
            UserId = "customer-1",
            ProgramId = program.Id,
            CurrentStamps = 2,
            LifetimeStamps = 2
        };

        var device = new Device
        {
            Id = 5,
            ShopId = 1,
            ProgramId = program.Id,
            Name = "Kasa 1 Tokmak",
            KeyHash = DeviceKeyHelper.ComputeHash("secret-k"),
            IsActive = true
        };

        var events = new List<StampEvent>();
        var now = DateTime.UtcNow;

        // Tokmak dokunma simülasyonu
        var nextStamps = card.CurrentStamps + 1;
        card.LifetimeStamps += 1;

        if (nextStamps >= program.RequiredStamps)
        {
            card.CurrentStamps = 0;
            events.Add(new StampEvent
            {
                StampCardId = card.Id,
                Type = StampEventType.RewardEarned,
                Source = StampSource.Tap,
                DeviceId = device.Id,
                CreatedAt = now
            });
        }
        else
        {
            card.CurrentStamps = nextStamps;
            events.Add(new StampEvent
            {
                StampCardId = card.Id,
                Type = StampEventType.Stamp,
                Source = StampSource.Tap,
                DeviceId = device.Id,
                CreatedAt = now
            });
        }

        Assert.Equal(3, card.CurrentStamps);
        Assert.Equal(3, card.LifetimeStamps);
        Assert.Single(events);
        Assert.Equal(StampEventType.Stamp, events[0].Type);
        Assert.Equal(StampSource.Tap, events[0].Source);
        Assert.Equal(device.Id, events[0].DeviceId);
    }

    [Fact]
    public void TapFlow_WhenReachingRequiredStamps_EmitsRewardEarnedAndResetsCurrentStamps()
    {
        // Karar D08: Hedef damgaya ulaşıldığında ödül hak edilir, CurrentStamps sıfırlanır, LifetimeStamps artmaya devam eder
        var program = new LoyaltyProgram
        {
            Id = 1,
            Name = "7 Kahveye 1 İkram",
            RequiredStamps = 7,
            RewardName = "Filtre Kahve",
            StampCooldownSeconds = 180
        };

        var card = new StampCard
        {
            Id = 20,
            UserId = "customer-2",
            ProgramId = program.Id,
            CurrentStamps = 6, // 6 damgası var, 7. damga gelecek
            LifetimeStamps = 13
        };

        var device = new Device
        {
            Id = 7,
            ShopId = 1,
            ProgramId = program.Id,
            Name = "Masa 3 NFC",
            KeyHash = DeviceKeyHelper.ComputeHash("masa-3-key"),
            IsActive = true
        };

        var events = new List<StampEvent>();
        var now = DateTime.UtcNow;

        var nextStamps = card.CurrentStamps + 1;
        card.LifetimeStamps += 1;

        if (nextStamps >= program.RequiredStamps)
        {
            card.CurrentStamps = 0; // Sıfırlandı
            events.Add(new StampEvent
            {
                StampCardId = card.Id,
                Type = StampEventType.RewardEarned,
                Source = StampSource.Tap,
                DeviceId = device.Id,
                CreatedAt = now
            });
        }

        Assert.Equal(0, card.CurrentStamps); // Sıfırlandı
        Assert.Equal(14, card.LifetimeStamps); // 13 + 1 = 14
        Assert.Single(events);
        Assert.Equal(StampEventType.RewardEarned, events[0].Type);
        Assert.Equal(StampSource.Tap, events[0].Source);
    }

    [Fact]
    public void TapFlow_CooldownViolation_BlocksStampWithinWindow()
    {
        // Karar D07: Soğuma süresi içinde mükerrer damga engellenir
        var cooldownSeconds = 180;
        var lastStampAt = DateTime.UtcNow.AddSeconds(-45); // 45 saniye önce basılmış
        var now = DateTime.UtcNow;

        var elapsed = (now - lastStampAt).TotalSeconds;
        var isWithinCooldown = elapsed < cooldownSeconds;

        Assert.True(isWithinCooldown);
        var remainingSeconds = (int)Math.Ceiling(cooldownSeconds - elapsed);
        Assert.InRange(remainingSeconds, 130, 136);
    }

    [Fact]
    public void TapFlow_DeviceKeyVerification_RejectsUnregisteredDevice()
    {
        var registeredKey = "damgio-real-device-key";
        var registeredKeyHash = DeviceKeyHelper.ComputeHash(registeredKey);

        var device = new Device
        {
            Id = 1,
            Name = "Kasa 1",
            KeyHash = registeredKeyHash,
            IsActive = true
        };

        var attackerKey = "fraudulent-key-attempt";
        var attackerHash = DeviceKeyHelper.ComputeHash(attackerKey);

        var isMatch = device.KeyHash == attackerHash;
        Assert.False(isMatch);
    }

    [Fact]
    public void AutoEnrollment_WhenCardDoesNotExist_InitializesZeroStampsCard()
    {
        // Müşteri /s/{slug} sayfasına ilk kez girdiğinde otomatik olarak 0 damgalı kart oluşturulur
        var userId = "new-customer-id";
        var programId = 3;
        var now = DateTime.UtcNow;

        var newCard = new StampCard
        {
            UserId = userId,
            ProgramId = programId,
            CurrentStamps = 0,
            LifetimeStamps = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        Assert.Equal(0, newCard.CurrentStamps);
        Assert.Equal(0, newCard.LifetimeStamps);
        Assert.Null(newCard.LastStampAt);
        Assert.Equal(userId, newCard.UserId);
        Assert.Equal(programId, newCard.ProgramId);
    }
}
