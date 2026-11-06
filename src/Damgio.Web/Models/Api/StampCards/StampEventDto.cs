namespace Damgio.Web.Models.Api.StampCards;

/// <summary>
/// Damga olayı (işlem geçmişi) DTO.
/// </summary>
public class StampEventDto
{
    /// <summary>Olay tekil kimliği.</summary>
    public long Id { get; set; }

    /// <summary>Damga kartı ID'si.</summary>
    public int StampCardId { get; set; }

    /// <summary>Olay türü (Stamp, RewardEarned, RewardUsed).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Olay kaynağı (Tap, Manual).</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>İşlemi yapan personel/kullanıcı ID'si (müşteri tap işleminde null).</summary>
    public string? ActorUserId { get; set; }

    /// <summary>İşlemi yapan personelin adı veya e-postası.</summary>
    public string? ActorUserName { get; set; }

    /// <summary>NFC cihaz ID'si (varsa).</summary>
    public int? DeviceId { get; set; }

    /// <summary>Olayın gerçekleştiği tarih ve saat (UTC).</summary>
    public DateTime CreatedAt { get; set; }
}
