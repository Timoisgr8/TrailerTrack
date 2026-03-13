using Microsoft.AspNetCore.Identity;

namespace TrailerTrack.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}