using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorPages.Data.Account;

namespace WebAppRazorPages.Pages.Account;

public class LogIn(SignInManager<User> signInManager) : PageModel
{
    [BindProperty]
    public CredentialViewModel CredentialViewModel { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await signInManager.PasswordSignInAsync(
            CredentialViewModel.Username,
            CredentialViewModel.Password,
            CredentialViewModel.RememberMe,
            false);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        ModelState.AddModelError("Login", result.IsLockedOut ? "You are locked out" : "Invalid login or password");

        return Page();
    }
}

public class CredentialViewModel
{
    [Required]
    [Display(Name = "User Name")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}