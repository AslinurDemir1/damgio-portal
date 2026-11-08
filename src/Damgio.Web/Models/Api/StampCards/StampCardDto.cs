namespace Damgio.Web.Models.Api.StampCards;

/// <summary>
/// Damga kartı veri transfer nesnesi (DTO).
/// </summary>
public class StampCardDto
{
    /// <summary>Kart tekil kimliği.</summary>
    public int Id { get; set; }

    /// <summary>Müşterinin kullanıcı ID'si (AspNetUsers.Id).</summary>
    public required string UserId { get; set; }

    /// <summary>Müşterinin adı soyadı.</summary>
    public string? UserFullName { get; set; }

    /// <summary>Müşterinin e-posta adresi.</summary>
    public string? UserEmail { get; set; }

    /// <summary>Bağlı olduğu sadakat programı ID'si.</summary>
    public int ProgramId { get; set; }

    /// <summary>Program adı (ör. 7 Kahveye 1 İkram).</summary>
    public string ProgramName { get; set; } = string.Empty;

    /// <summary>İşletme ID'si.</summary>
    public int ShopId { get; set; }

    /// <summary>İşletme adı (ör. Kadıköy Damgio Cafe).</summary>
    public string ShopName { get; set; } = string.Empty;

    /// <summary>İşletme URL slug'ı (ör. kadikoy-cafe).</summary>
    public string ShopSlug { get; set; } = string.Empty;

    /// <summary>Mevcut damga sayısı (ödül döngüsü tamamlandığında sıfırlanır).</summary>
    public int CurrentStamps { get; set; }

    /// <summary>Ödül için gereken toplam damga sayısı.</summary>
    public int RequiredStamps { get; set; }

    /// <summary>Kazanılan ödül adı.</summary>
    public string RewardName { get; set; } = string.Empty;

    /// <summary>Kullanıcının bu kartta toplam kazandığı ömür boyu damga sayısı.</summary>
    public int LifetimeStamps { get; set; }

    /// <summary>Son damga basılma zamanı (UTC).</summary>
    public DateTime? LastStampAt { get; set; }

    /// <summary>Kart oluşturulma tarihi (UTC).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Son güncellenme tarihi (UTC).</summary>
    public DateTime UpdatedAt { get; set; }
}
