namespace Damgio.Core.Entities;

/// <summary>A physical shop / branch that runs one or more loyalty programs.</summary>
public class Shop
{
    public int Id { get; set; }
    public required string Name { get; set; }

    /// <summary>Public URL segment: /s/{Slug}. Unique, lowercase.</summary>
    public required string Slug { get; set; }

    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<LoyaltyProgram> Programs { get; set; } = new List<LoyaltyProgram>();
    public ICollection<ShopMember> Members { get; set; } = new List<ShopMember>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();
}
