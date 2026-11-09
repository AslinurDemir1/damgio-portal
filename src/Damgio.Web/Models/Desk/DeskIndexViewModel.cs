
namespace Damgio.Web.Models.Desk;

public class DeskIndexViewModel
{
    // Seçilen işletme
    public int? SelectedShopId { get; set; }

    // Arama kutusuna yazılan telefon, e-posta veya kart numarası
    public string? SearchQuery { get; set; }

    // Kullanıcının erişebileceği işletmeler
    public List<DeskShopViewModel> Shops { get; set; } = new();

    // Arama sonucunda bulunan müşteri kartları
    public List<DeskCardViewModel> Cards { get; set; } = new();
}

public class DeskShopViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class DeskCardViewModel
{
    public int CardId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string ShopName { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string RewardName { get; set; } = string.Empty;

    public int CurrentStamps { get; set; }
    public int RequiredStamps { get; set; }
    public int LifetimeStamps { get; set; }

    public int AvailableRewards { get; set; }
}
