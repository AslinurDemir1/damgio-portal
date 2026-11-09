using System.Security.Cryptography;
using System.Text;

namespace Damgio.Core.Common;

/// <summary>
/// NFC sayaç cihazlarının (tokmak) tek yönlü SHA-256 anahtar doğrulamasını sağlar.
/// Karar D09 gereği fiziksel cihazdaki anahtar (k) veritabanında asla açık metin olarak tutulmaz,
/// yalnızca SHA-256 özeti saklanır.
/// </summary>
public static class DeviceKeyHelper
{
    /// <summary>
    /// Ham anahtar metninden 64 karakterlik küçük harf SHA-256 özetini üretir.
    /// </summary>
    public static string ComputeHash(string rawKey)
    {
        if (string.IsNullOrEmpty(rawKey))
        {
            return string.Empty;
        }

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    /// <summary>
    /// İstemciden gelen ham anahtarın veritabanındaki KeyHash ile eşleşip eşleşmediğini doğrular.
    /// </summary>
    public static bool VerifyKey(string rawKey, string expectedHash)
    {
        if (string.IsNullOrEmpty(rawKey) || string.IsNullOrEmpty(expectedHash))
        {
            return false;
        }

        var computed = ComputeHash(rawKey);
        return string.Equals(computed, expectedHash, StringComparison.OrdinalIgnoreCase);
    }
}
