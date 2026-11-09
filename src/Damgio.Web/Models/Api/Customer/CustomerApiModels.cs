using System.ComponentModel.DataAnnotations;

namespace Damgio.Web.Models.Api.Customer;

/// <summary>
/// Müşteri NFC / QR tokmak dokunma isteği modeli.
/// </summary>
public class TapRequest
{
    /// <summary>
    /// İşletmenin tekil URL slug değeri (ör. 'kadikoy-damgio-cafe').
    /// </summary>
    [Required(ErrorMessage = "İşletme slug değeri zorunludur.")]
    public required string Slug { get; set; }

    /// <summary>
    /// Fiziksel NFC sayaç cihazının ürettiği gizli anahtar (k parametresi).
    /// </summary>
    [Required(ErrorMessage = "Cihaz anahtarı (key) zorunludur.")]
    public required string Key { get; set; }
}

/// <summary>
/// NFC / QR tokmak dokunma işlem sonucu yanıtı.
/// </summary>
public class TapResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool RewardEarned { get; set; }
    public int CurrentStamps { get; set; }
    public int RequiredStamps { get; set; }
    public int LifetimeStamps { get; set; }
    public string RewardName { get; set; } = string.Empty;
    public int CardId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public int RemainingCooldownSeconds { get; set; }
}

/// <summary>
/// Müşterinin sahip olduğu damga kartının mobil ve API özeti.
/// </summary>
public class CustomerCardSummaryDto
{
    public int CardId { get; set; }
    public int ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public string ShopSlug { get; set; } = string.Empty;
    public int ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string RewardName { get; set; } = string.Empty;
    public int CurrentStamps { get; set; }
    public int RequiredStamps { get; set; }
    public int LifetimeStamps { get; set; }
    public DateTime? LastStampAt { get; set; }
    public bool CanStampNow { get; set; }
    public int CooldownSeconds { get; set; }
}
