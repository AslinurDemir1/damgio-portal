namespace Damgio.Core.Entities;

/// <summary>
/// A stamp-card rule set ("7 coffees, 8th free"). Stored in the Programs table.
/// Named LoyaltyProgram to avoid clashing with the ASP.NET Core Program class.
/// </summary>
public class LoyaltyProgram
{
    public int Id { get; set; }
    public int ShopId { get; set; }
    public required string Name { get; set; }
    public int RequiredStamps { get; set; } = 7;
    public required string RewardName { get; set; }
    public int StampCooldownSeconds { get; set; } = 180;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Shop? Shop { get; set; }
    public ICollection<StampCard> StampCards { get; set; } = new List<StampCard>();
}
