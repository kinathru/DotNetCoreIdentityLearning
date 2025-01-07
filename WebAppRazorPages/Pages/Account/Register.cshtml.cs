using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppRazorPages.Pages.Account;

public class Register : PageModel
{
    private readonly UserManager<IdentityUser> userManager;

    [BindProperty]
    public RegisterViewModel RegisterViewModel { get; set; } = new();

    public Register(UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Validate email address (optional)

        // Create the user
        var user = new IdentityUser()
        {
            Email = RegisterViewModel.Email,
            UserName = RegisterViewModel.Email
        };

        var result = await userManager.CreateAsync(user, RegisterViewModel.Password);
        if (result.Succeeded)
        {
            var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            
            return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = emailConfirmationToken }) ?? string.Empty);
        }

        foreach (var err in result.Errors)
        {
            ModelState.AddModelError("Register", err.Description);
        }

        return Page();
    }
}

public class RegisterViewModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}