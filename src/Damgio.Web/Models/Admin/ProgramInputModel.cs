using System.ComponentModel.DataAnnotations;

namespace Damgio.Web.Models.Admin;

public class ProgramInputModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Lütfen bir işletme seçiniz.")]
    [Display(Name = "İşletme")]
    public int ShopId { get; set; }

    [Required(ErrorMessage = "Program adı zorunludur.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Program adı 2 ile 100 karakter arasında olmalıdır.")]
    [Display(Name = "Program Adı")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gerekli damga sayısı zorunludur.")]
    [Range(1, 50, ErrorMessage = "Gerekli damga sayısı 1 ile 50 arasında olmalıdır.")]
    [Display(Name = "Gerekli Damga Sayısı")]
    public int RequiredStamps { get; set; } = 7;

    [Required(ErrorMessage = "Kazanılacak ödül açıklaması zorunludur.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ödül açıklaması 2 ile 100 karakter arasında olmalıdır.")]
    [Display(Name = "Kazanılacak Ödül")]
    public string RewardName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Damga soğuma süresi zorunludur.")]
    [Range(0, 86400, ErrorMessage = "Soğuma süresi 0 ile 86400 saniye (24 saat) arasında olmalıdır.")]
    [Display(Name = "Soğuma Süresi (Saniye)")]
    public int StampCooldownSeconds { get; set; } = 180;

    [Display(Name = "Aktif mi?")]
    public bool IsActive { get; set; } = true;
}
