using System.ComponentModel.DataAnnotations;

namespace Damgio.Web.Models.Api.StampCards;

/// <summary>
/// Yeni damga kartı oluşturma isteği.
/// </summary>
public class CreateStampCardRequest
{
    /// <summary>Kart tanımlanacak müşterinin kullanıcı ID'si (AspNetUsers.Id).</summary>
    [Required(ErrorMessage = "Kullanıcı ID (UserId) zorunludur.")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>Bağlanacak sadakat programının ID'si.</summary>
    [Required(ErrorMessage = "Program ID (ProgramId) zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Program ID girilmelidir.")]
    public int ProgramId { get; set; }

    /// <summary>Kart oluşturulurken tanımlanacak başlangıç damga sayısı (varsayılan: 0).</summary>
    [Range(0, 50, ErrorMessage = "Başlangıç damga sayısı 0 ile 50 arasında olmalıdır.")]
    public int InitialStamps { get; set; } = 0;
}
