using System.ComponentModel.DataAnnotations;
using Damgio.Core.Enums;

namespace Damgio.Web.Models.Admin;

/// <summary>
/// İşletmeye personel (Owner / Cashier) atama girdi modeli.
/// </summary>
public class ShopMemberInputModel
{
    [Required]
    public int ShopId { get; set; }

    [Required(ErrorMessage = "Personel e-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string UserEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rol seçimi zorunludur.")]
    public ShopRole Role { get; set; } = ShopRole.Cashier;
}
