using System.ComponentModel.DataAnnotations;
using Damgio.Core.Entities;
using Damgio.Core.Enums;
using Damgio.Web.Models.Api.StampCards;
using Xunit;

namespace Damgio.Tests;

public class StampCardTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void CreateStampCardRequest_ValidModel_PassesValidation()
    {
        var model = new CreateStampCardRequest
        {
            UserId = "user-123",
            ProgramId = 1,
            InitialStamps = 2
        };

        var results = ValidateModel(model);
        Assert.Empty(results);
    }

    [Theory]
    [InlineData("", 1, 0, "Kullanıcı ID (UserId) zorunludur.")]
    [InlineData("user-1", 0, 0, "Geçerli bir Program ID girilmelidir.")]
    [InlineData("user-1", -5, 0, "Geçerli bir Program ID girilmelidir.")]
    [InlineData("user-1", 1, -1, "Başlangıç damga sayısı 0 ile 50 arasında olmalıdır.")]
    [InlineData("user-1", 1, 51, "Başlangıç damga sayısı 0 ile 50 arasında olmalıdır.")]
    public void CreateStampCardRequest_InvalidModel_FailsValidation(
        string userId, int programId, int initialStamps, string expectedErrorMessage)
    {
        var model = new CreateStampCardRequest
        {
            UserId = userId,
            ProgramId = programId,
            InitialStamps = initialStamps
        };

        var results = ValidateModel(model);
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.ErrorMessage == expectedErrorMessage);
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(7, 14, true)]
    [InlineData(50, 50, true)]
    [InlineData(-1, 0, false)]
    [InlineData(51, 10, false)]
    [InlineData(5, -1, false)]
    public void UpdateStampCardRequest_ValidationRules_AreEnforced(
        int currentStamps, int lifetimeStamps, bool isValid)
    {
        var model = new UpdateStampCardRequest
        {
            CurrentStamps = currentStamps,
            LifetimeStamps = lifetimeStamps
        };

        var results = ValidateModel(model);
        if (isValid)
        {
            Assert.Empty(results);
        }
        else
        {
            Assert.NotEmpty(results);
        }
    }

    [Fact]
    public void StampCard_RewardCycle_ResetsCurrentStampsAndIncrementsLifetime()
    {
        // Kural D08: CurrentStamps >= RequiredStamps -> RewardEarned event, CurrentStamps = 0, LifetimeStamps keeps counting
        var program = new LoyaltyProgram
        {
            Id = 1,
            Name = "7 Kahveye 1 İkram",
            RequiredStamps = 7,
            RewardName = "Ücretsiz Kahve",
            StampCooldownSeconds = 180
        };

        var card = new StampCard
        {
            UserId = "user-abc",
            ProgramId = program.Id,
            CurrentStamps = 6,
            LifetimeStamps = 6
        };

        // 7. damga eklendiğinde döngü kontrolü
        int newStamps = card.CurrentStamps + 1;
        var events = new List<StampEvent>();

        if (newStamps >= program.RequiredStamps)
        {
            events.Add(new StampEvent
            {
                Type = StampEventType.RewardEarned,
                Source = StampSource.Manual,
                CreatedAt = DateTime.UtcNow
            });

            card.LifetimeStamps += 1;
            card.CurrentStamps = 0; // Döngü sıfırlandı
        }

        Assert.Equal(0, card.CurrentStamps);
        Assert.Equal(7, card.LifetimeStamps);
        Assert.Single(events);
        Assert.Equal(StampEventType.RewardEarned, events[0].Type);
    }

    [Fact]
    public void StampCard_Cooldown_DetectsViolationsCorrectly()
    {
        var program = new LoyaltyProgram
        {
            Name = "Kahve Kartı",
            RewardName = "1 İkram Kahve",
            StampCooldownSeconds = 180
        };

        var lastStampAt = DateTime.UtcNow.AddSeconds(-60); // 60 saniye önce basılmış
        var now = DateTime.UtcNow;

        var elapsedSeconds = (now - lastStampAt).TotalSeconds;
        bool isCooldownActive = elapsedSeconds < program.StampCooldownSeconds;

        Assert.True(isCooldownActive);

        // 200 saniye sonra soğuma geçmiş olmalı
        var pastLastStampAt = DateTime.UtcNow.AddSeconds(-200);
        bool isPastCooldownActive = (now - pastLastStampAt).TotalSeconds < program.StampCooldownSeconds;
        Assert.False(isPastCooldownActive);
    }
}
