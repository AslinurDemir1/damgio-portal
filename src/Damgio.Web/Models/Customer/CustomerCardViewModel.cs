namespace Damgio.Web.Models.Customer;

public class CustomerCardViewModel
{
    public int? CardId { get; set; }
    public required string ShopName { get; set; }
    public required string ShopSlug { get; set; }
    public string? ShopAddress { get; set; }
    public required string ProgramName { get; set; }
    public int ProgramId { get; set; }
    public int CurrentStamps { get; set; }
    public int RequiredStamps { get; set; } = 7;
    public int LifetimeStamps { get; set; }
    public required string RewardName { get; set; }
    public int CooldownSeconds { get; set; } = 180;
    public DateTime? LastStampAt { get; set; }

    public bool IsAuthenticated { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }

    public bool CanStampNow { get; set; } = true;
    public int RemainingCooldownSeconds { get; set; }
    public bool RewardEarnedNotice { get; set; }

    public string? StatusMessage { get; set; }
    public string? WarningMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public IReadOnlyList<CustomerStampEventItem> RecentEvents { get; set; } = [];

    public int StampsRemaining => Math.Max(0, RequiredStamps - CurrentStamps);
    public double ProgressPercentage => RequiredStamps > 0
        ? Math.Min(100.0, Math.Round((double)CurrentStamps / RequiredStamps * 100, 1))
        : 0;
}

public class CustomerStampEventItem
{
    public long Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; } = string.Empty;
}
