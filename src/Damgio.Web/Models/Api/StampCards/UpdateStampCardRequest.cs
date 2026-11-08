using System.ComponentModel.DataAnnotations;

namespace Damgio.Web.Models.Api.StampCards;

/// <summary>
/// Mevcut damga kartını güncelleme isteği.
/// </summary>
public class UpdateStampCardRequest
{
    /// <summary>Karttaki güncel damga sayısı.</summary>
    [Required(ErrorMessage = "Mevcut damga sayısı (CurrentStamps) zorunludur.")]
    [Range(0, 50, ErrorMessage = "Mevcut damga sayısı 0 ile 50 arasında olmalıdır.")]
    public int CurrentStamps { get; set; }

    /// <summary>Kullanıcının bu programda kazandığı toplam ömür boyu damga sayısı (boş bırakılırsa otomatik hesaplanır veya korunur).</summary>
    [Range(0, 100000, ErrorMessage = "Toplam damga sayısı negatif olamaz.")]
    public int? LifetimeStamps { get; set; }
}
