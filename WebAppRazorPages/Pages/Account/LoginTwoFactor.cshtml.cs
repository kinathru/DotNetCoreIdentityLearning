using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorPages.Data.Account;
using WebAppRazorPages.Services;

namespace WebAppRazorPages.Pages.Account;

public class LoginTwoFactor : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;

    public LoginTwoFactor(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailService emailService)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._emailService = emailService;
        EmailMFA = new EmailMFA();
    }

    [BindProperty]
    public EmailMFA EmailMFA { get; set; }

    public async Task OnGetAsync(string userName, bool rememberMe)
    {
        var user = await _userManager.FindByNameAsync(userName);
        this.EmailMFA.RememberMe = rememberMe;
        if (user == null)
        {
            ModelState.AddModelError("2-Factor", "Unable to load user with UserName.");
            return;
        }

        if (user.Email == null)
        {
            ModelState.AddModelError("2-Factor", "No email address provided.");
            return;
        }

        // Generate the code
        var securityCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

        // Send to the user
        await _emailService.SendAsync("kinathru@gmail.com", user.Email, "My Web App's OTP",
            $"Please use this code as the OTP: {securityCode}");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result =
            await _signInManager.TwoFactorSignInAsync("Email", EmailMFA.SecurityCode, EmailMFA.RememberMe, false);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("Login 2FA", "This user is locked out.");
        }
        else
        {
            ModelState.AddModelError("Login 2FA", "Failed to login");
        }

        return Page();
    }
}

public class EmailMFA
{
    [Required]
    [Display(Name = "Security Code")]
    public string SecurityCode { get; set; }

    public bool RememberMe { get; set; }
}