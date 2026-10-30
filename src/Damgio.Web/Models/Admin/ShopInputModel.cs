using System.ComponentModel.DataAnnotations;

namespace Damgio.Web.Models.Admin;

public class ShopInputModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "İşletme adı zorunludur.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "İşletme adı 2 ile 100 karakter arasında olmalıdır.")]
    [Display(Name = "İşletme Adı")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Bağlantı Adresi (Slug)")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Slug 2 ile 50 karakter arasında olmalıdır.")]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug yalnızca küçük harf, rakam ve tek tire içerebilir (Örn: kadikoy-cafe).")]
    public string? Slug { get; set; }

    [Display(Name = "Adres")]
    [StringLength(250, ErrorMessage = "Adres en fazla 250 karakter olabilir.")]
    public string? Address { get; set; }

    [Display(Name = "Aktif mi?")]
    public bool IsActive { get; set; } = true;
}
