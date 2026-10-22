namespace Damgio.Core.Constants;

/// <summary>Application-wide Identity role names.</summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Cashier = "Cashier";
    public const string Customer = "Customer";

    public static readonly string[] All = [Admin, Cashier, Customer];
}
