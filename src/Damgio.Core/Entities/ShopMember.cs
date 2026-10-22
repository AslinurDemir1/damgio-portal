using Damgio.Core.Enums;

namespace Damgio.Core.Entities;

/// <summary>Links a staff user to a shop with a role. Unique per (ShopId, UserId).</summary>
public class ShopMember
{
    public int Id { get; set; }
    public int ShopId { get; set; }
    public required string UserId { get; set; }
    public ShopRole Role { get; set; }
    public DateTime CreatedAt { get; set; }

    public Shop? Shop { get; set; }
}
