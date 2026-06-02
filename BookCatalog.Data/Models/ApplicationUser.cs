using Microsoft.AspNetCore.Identity;

namespace BookCatalog.Data.Models;

// Inherits everything IdentityUser gives us (Id, Email, PasswordHash, etc.)
// and adds a custom property. This satisfies the "add custom properties
// for the user" requirement. Inheritance in action: ApplicationUser IS an
// IdentityUser plus a bit more.
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
