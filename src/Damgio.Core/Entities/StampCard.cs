namespace Damgio.Core.Entities;

/// <summary>One customer's card in one program. Unique per (UserId, ProgramId).</summary>
public class StampCard
{
    public int Id { get; set; }

    /// <summary>Identity user id (AspNetUsers.Id) of the customer.</summary>
    public required string UserId { get; set; }

    public int ProgramId { get; set; }
    public int CurrentStamps { get; set; }
    public int LifetimeStamps { get; set; }
    public DateTime? LastStampAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public LoyaltyProgram? Program { get; set; }
    public ICollection<StampEvent> Events { get; set; } = new List<StampEvent>();
}
