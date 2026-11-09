using Damgio.Core.Common;
using Xunit;

namespace Damgio.Tests;

public class DeviceKeyHelperTests
{
    [Fact]
    public void ComputeHash_WithValidString_Returns64CharacterLowercaseHex()
    {
        var rawKey = "damgio-tokmak-secret-2026";
        var hash = DeviceKeyHelper.ComputeHash(rawKey);

        Assert.NotNull(hash);
        Assert.Equal(64, hash.Length);
        Assert.Equal(hash.ToLowerInvariant(), hash);
    }

    [Fact]
    public void ComputeHash_WithKnownKey_MatchesExpectedSha256()
    {
        // SHA-256 for "test-key" is 9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08
        var rawKey = "test";
        var hash = DeviceKeyHelper.ComputeHash(rawKey);

        // SHA-256("test") = 9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08
        Assert.Equal("9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", hash);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ComputeHash_WithNullOrEmpty_ReturnsEmptyString(string? rawKey)
    {
        var hash = DeviceKeyHelper.ComputeHash(rawKey!);
        Assert.Equal(string.Empty, hash);
    }

    [Fact]
    public void VerifyKey_WithMatchingKeyAndHash_ReturnsTrue()
    {
        var rawKey = "tokmak-kasa-1";
        var hash = DeviceKeyHelper.ComputeHash(rawKey);

        var isValid = DeviceKeyHelper.VerifyKey(rawKey, hash);
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyKey_WithIncorrectKey_ReturnsFalse()
    {
        var rawKey = "correct-secret";
        var hash = DeviceKeyHelper.ComputeHash(rawKey);

        var isValid = DeviceKeyHelper.VerifyKey("wrong-secret", hash);
        Assert.False(isValid);
    }

    [Fact]
    public void VerifyKey_WithUppercaseHash_MatchesCaseInsensitively()
    {
        var rawKey = "secret-token-42";
        var lowercaseHash = DeviceKeyHelper.ComputeHash(rawKey);
        var uppercaseHash = lowercaseHash.ToUpperInvariant();

        var isValid = DeviceKeyHelper.VerifyKey(rawKey, uppercaseHash);
        Assert.True(isValid);
    }

    [Theory]
    [InlineData(null, "somehash")]
    [InlineData("somekey", null)]
    [InlineData("", "")]
    public void VerifyKey_WithNullOrEmptyInputs_ReturnsFalse(string? key, string? hash)
    {
        var isValid = DeviceKeyHelper.VerifyKey(key!, hash!);
        Assert.False(isValid);
    }
}
