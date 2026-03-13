using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrailerTrack.Infrastructure.Identity;

namespace TrailerTrack.Web.Pages;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public LoginModel(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _signInManager.PasswordSignInAsync(
            Email, Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
            return Redirect("/assets");

        ErrorMessage = "Invalid email or password.";
        return Page();
    }
}