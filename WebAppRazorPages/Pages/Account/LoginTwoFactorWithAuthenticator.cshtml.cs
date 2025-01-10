using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorPages.Data.Account;

namespace WebAppRazorPages.Pages.Account;

public class LoginTwoFactorWithAuthenticator : PageModel
{
    private readonly SignInManager<User> _signInManager;

    [BindProperty]
    public AuthenticatorMFAViewModel AuthenticatorMfa { get; set; }

    public LoginTwoFactorWithAuthenticator(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
        AuthenticatorMfa = new AuthenticatorMFAViewModel();
    }

    public void OnGet(bool rememberMe)
    {
        AuthenticatorMfa.SecurityCode = string.Empty;
        AuthenticatorMfa.RememberMe = rememberMe;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMfa.SecurityCode,
            AuthenticatorMfa.RememberMe,
            false);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("Authenticator 2FA", "This user is locked out.");
        }
        else
        {
            ModelState.AddModelError("Authenticator 2FA", "Failed to login");
        }

        return Page();
    }
}

public class AuthenticatorMFAViewModel
{
    [Required]
    [Display(Name = "Code")]
    public string SecurityCode { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}