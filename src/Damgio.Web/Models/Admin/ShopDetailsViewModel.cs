using Damgio.Core.Entities;

namespace Damgio.Web.Models.Admin;

/// <summary>
/// İşletme detay sayfası için birleşik model (İşletme, Programlar, Personel listesi ve Yeni Personel ekleme formu).
/// </summary>
public class ShopDetailsViewModel
{
    public required Shop Shop { get; set; }
    public List<ShopMemberViewModel> Members { get; set; } = new();
    public ShopMemberInputModel NewMember { get; set; } = new();
}
