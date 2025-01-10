using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorPages.Data.Account;

namespace WebAppRazorPages.Pages.Account;

[Authorize]
public class AuthenticatorWithMFASetup : PageModel
{
    private readonly UserManager<User> _userManager;

    [BindProperty]
    public SetupMfaViewModel ViewModel { get; set; }
    
    [BindProperty]
    public bool Succeeded { get; set; }

    public AuthenticatorWithMFASetup(UserManager<User> userManager)
    {
        _userManager = userManager;
        ViewModel = new SetupMfaViewModel();
        Succeeded = false;
    }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(base.User);
        if (user == null)
        {
            ModelState.AddModelError("MFA Setup", "User not found");
            return;
        }

        // Allow the user to generate the security key everytime they come to MFA setup page
        await _userManager.ResetAuthenticatorKeyAsync(user);
        var key = await _userManager.GetAuthenticatorKeyAsync(user);

        ViewModel.Key = key ?? string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(base.User);
        if (user == null)
        {
            ModelState.AddModelError("MFA Setup", "User not found");
            return Page();
        }

        var isVerified = await _userManager.VerifyTwoFactorTokenAsync(user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            this.ViewModel.SecurityCode);

        if (isVerified)
        {
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            Succeeded = true;
        }
        else
        {
            ModelState.AddModelError("MFA Setup", "Something went wrong with the authenticator setup.");
            Succeeded = false;
        }
        
        return Page();
    }
}

public class SetupMfaViewModel
{
    public string? Key { get; set; }

    [Required]
    [Display(Name = "Code")]
    public string SecurityCode { get; set; }
}