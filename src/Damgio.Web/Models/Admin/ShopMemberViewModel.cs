using Damgio.Core.Enums;

namespace Damgio.Web.Models.Admin;

/// <summary>
/// İşletmeye bağlı personel görüntüleme modeli.
/// </summary>
public class ShopMemberViewModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public ShopRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
