namespace Damgio.Web.Models.Api.Common;

/// <summary>
/// Standart API hata yanıt formatı.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>Damgio API standart hata kodu (ör. NOT_FOUND, VALIDATION, ALREADY_EXISTS).</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Kullanıcı dostu Türkçe hata açıklaması.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Detaylı alan hataları listesi (validasyon hataları için).</summary>
    public IDictionary<string, string[]>? Details { get; set; }

    public ApiErrorResponse() { }

    public ApiErrorResponse(string code, string message, IDictionary<string, string[]>? details = null)
    {
        Code = code;
        Message = message;
        Details = details;
    }
}
