using Damgio.Core.Common;
using Xunit;

namespace Damgio.Tests;

public class SlugHelperTests
{
    [Theory]
    [InlineData("Kadıköy Damgio Cafe", "kadikoy-damgio-cafe")]
    [InlineData("Çiçek & Güneş Fırını", "cicek-gunes-firini")]
    [InlineData("Şişli Örnek Çarşı Kahvecisi", "sisli-ornek-carsi-kahvecisi")]
    [InlineData("İSTANBUL KAHVESİ", "istanbul-kahvesi")]
    [InlineData("  Espresso   Lab -- Kadıköy  ", "espresso-lab-kadikoy")]
    [InlineData("Damgio's Cafe #1 (Beşiktaş)", "damgio-s-cafe-1-besiktas")]
    public void ToSlug_ConvertsTurkishAndSpecialCharacters_Successfully(string input, string expected)
    {
        var result = SlugHelper.ToSlug(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("---")]
    [InlineData("!@#$%^&*()")]
    public void ToSlug_ReturnsEmpty_ForEmptyOrSpecialOnlyInput(string? input)
    {
        var result = SlugHelper.ToSlug(input);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("Admin")]
    [InlineData("ADMIN")]
    [InlineData("api")]
    [InlineData("desk")]
    [InlineData("s")]
    [InlineData("account")]
    [InlineData("login")]
    [InlineData("swagger")]
    public void IsReservedSlug_ReturnsTrue_ForReservedSlugs(string slug)
    {
        Assert.True(SlugHelper.IsReservedSlug(slug));
    }

    [Theory]
    [InlineData("kadikoy-cafe")]
    [InlineData("damgio-moda")]
    [InlineData("besiktas-kahvecisi")]
    [InlineData("shop")]
    public void IsReservedSlug_ReturnsFalse_ForNonReservedSlugs(string slug)
    {
        Assert.False(SlugHelper.IsReservedSlug(slug));
    }

    [Theory]
    [InlineData("kadikoy-cafe", true)]
    [InlineData("damgio123", true)]
    [InlineData("cafe-7-plus-1", true)]
    [InlineData("a", false)]                     // 1 karakter (çok kısa, min 2)
    [InlineData("admin", false)]                 // Rezerve rota
    [InlineData("swagger", false)]               // Rezerve rota
    [InlineData("kadikoy--cafe", false)]         // Çift tire geçersiz
    [InlineData("-kadikoy", false)]              // Başta tire geçersiz
    [InlineData("kadikoy-", false)]              // Sonda tire geçersiz
    [InlineData("Kadikoy-Cafe", false)]          // Büyük harf geçersiz
    [InlineData("kadıköy", false)]               // ASCII dışı Türkçe karakter doğrudan geçersiz (slug dönüşmeli)
    [InlineData("", false)]                      // Boş
    [InlineData(null, false)]                    // Null
    public void IsValidSlug_ValidatesCorrectly(string? slug, bool expected)
    {
        var result = SlugHelper.IsValidSlug(slug);
        Assert.Equal(expected, result);
    }
}
