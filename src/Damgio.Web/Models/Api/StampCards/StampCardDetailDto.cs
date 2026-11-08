namespace Damgio.Web.Models.Api.StampCards;

/// <summary>
/// Damga kartı detay DTO'su (olay geçmişi ile birlikte).
/// </summary>
public class StampCardDetailDto : StampCardDto
{
    /// <summary>Karta ait son damga ve ödül olayları.</summary>
    public List<StampEventDto> Events { get; set; } = new();
}
