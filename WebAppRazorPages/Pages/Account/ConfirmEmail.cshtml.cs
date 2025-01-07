using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppRazorPages.Pages.Account;

public class ConfirmEmail(UserManager<IdentityUser> userManager) : PageModel
{
    [BindProperty]
    public string Message { get; set; } = string.Empty;
    
    public async Task<IActionResult> OnGetAsync(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                Message = "Email address is successfully confirmed, you can now login.";
                return Page();
            }
        }
        
        Message = "failed to validate email";
        return Page();
    }
}