namespace Damgio.Core.Entities;

/// <summary>
/// NFC counter tag ("tokmak"). The tag holds /s/{slug}/tap?k={key};
/// only the SHA-256 hash of the key is stored.
/// </summary>
public class Device
{
    public int Id { get; set; }
    public int ShopId { get; set; }
    public int ProgramId { get; set; }
    public required string Name { get; set; }
    public required string KeyHash { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? RotatedAt { get; set; }

    public Shop? Shop { get; set; }
    public LoyaltyProgram? Program { get; set; }
}
