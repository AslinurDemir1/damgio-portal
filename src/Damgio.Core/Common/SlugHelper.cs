using System.Text;
using System.Text.RegularExpressions;

namespace Damgio.Core.Common;

/// <summary>
/// Utility for generating and validating URL-friendly slugs for shops and public resources.
/// Handles Turkish character transliteration and enforces reserved slug constraints.
/// </summary>
public static partial class SlugHelper
{
    /// <summary>
    /// Reserved URL prefixes and segments that cannot be used as shop slugs (ENGINEERING.md §4).
    /// </summary>
    public static readonly string[] ReservedSlugs =
    [
        "admin",
        "api",
        "desk",
        "s",
        "account",
        "login",
        "swagger"
    ];

    private static readonly HashSet<string> ReservedSet =
        new(ReservedSlugs, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Generates a clean, lowercase, URL-safe slug from a given text.
    /// Converts Turkish characters (ç, ğ, ı, ö, ş, ü) to ASCII equivalents.
    /// </summary>
    public static string ToSlug(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // 1. Transliterate Turkish characters explicitly
        var sb = new StringBuilder(text.Length);
        foreach (var ch in text)
        {
            switch (ch)
            {
                case 'ı':
                case 'İ':
                case 'I':
                    sb.Append('i');
                    break;
                case 'ğ':
                case 'Ğ':
                    sb.Append('g');
                    break;
                case 'ü':
                case 'Ü':
                    sb.Append('u');
                    break;
                case 'ş':
                case 'Ş':
                    sb.Append('s');
                    break;
                case 'ö':
                case 'Ö':
                    sb.Append('o');
                    break;
                case 'ç':
                case 'Ç':
                    sb.Append('c');
                    break;
                default:
                    sb.Append(ch);
                    break;
            }
        }

        var normalized = sb.ToString().ToLowerInvariant();

        // 2. Replace non-alphanumeric characters with hyphens
        var cleaned = Regex.Replace(normalized, @"[^a-z0-9]+", "-");

        // 3. Trim leading and trailing hyphens
        cleaned = cleaned.Trim('-');

        return cleaned;
    }

    /// <summary>
    /// Checks whether the specified slug matches system-reserved routing segments.
    /// </summary>
    public static bool IsReservedSlug(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return false;

        return ReservedSet.Contains(slug.Trim());
    }

    /// <summary>
    /// Validates whether a slug conforms to URL format rules (lowercase alphanumeric with hyphens, not reserved, max length 50).
    /// </summary>
    public static bool IsValidSlug(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return false;

        slug = slug.Trim();

        if (slug.Length is < 2 or > 50)
            return false;

        if (IsReservedSlug(slug))
            return false;

        return Regex.IsMatch(slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$");
    }
}
