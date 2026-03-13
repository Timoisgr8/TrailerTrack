using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrailerTrack.Infrastructure.Identity;

namespace TrailerTrack.Web.Pages;

public class LogoutModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public LogoutModel(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/login");
    }
}