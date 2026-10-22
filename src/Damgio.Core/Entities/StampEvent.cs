using Damgio.Core.Enums;

namespace Damgio.Core.Entities;

/// <summary>Immutable history row: a stamp, an earned reward or a used reward.</summary>
public class StampEvent
{
    public long Id { get; set; }
    public int StampCardId { get; set; }
    public StampEventType Type { get; set; }
    public StampSource Source { get; set; }

    /// <summary>Staff user who performed the action (null for NFC tap by the customer).</summary>
    public string? ActorUserId { get; set; }

    /// <summary>NFC device that produced the stamp, if any.</summary>
    public int? DeviceId { get; set; }

    public DateTime CreatedAt { get; set; }

    public StampCard? StampCard { get; set; }
    public Device? Device { get; set; }
}
