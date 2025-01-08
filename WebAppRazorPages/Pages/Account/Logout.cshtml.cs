using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppRazorPages.Pages.Account;

public class Logout : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public Logout(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Account/Login");
    }
}