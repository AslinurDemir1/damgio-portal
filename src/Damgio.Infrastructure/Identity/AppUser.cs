using Microsoft.AspNetCore.Identity;

namespace Damgio.Infrastructure.Identity;

/// <summary>Identity user for every actor (admin, cashier, customer).</summary>
public class AppUser : IdentityUser
{
    public required string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
}
