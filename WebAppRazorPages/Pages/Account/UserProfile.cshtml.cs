using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorPages.Data.Account;

namespace WebAppRazorPages.Pages.Account;

public class UserProfile(UserManager<User> userManager) : PageModel
{
    [BindProperty]
    public UserProfileViewModel ProfileViewModel { get; set; } = new();
    
    [BindProperty]
    public string? SuccessMessage { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        SuccessMessage = string.Empty;
        var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();

        if (user != null)
        {
            ProfileViewModel.Email = User.Identity?.Name ?? string.Empty;
            ProfileViewModel.Department = departmentClaim?.Value ?? string.Empty;
            ProfileViewModel.Position = positionClaim?.Value ?? string.Empty;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();

            if (user != null && departmentClaim != null)
            {
                await userManager.ReplaceClaimAsync(user, departmentClaim,
                    new Claim(departmentClaim.Type, ProfileViewModel.Department));
            }

            if (user != null && positionClaim != null)
            {
                await userManager.ReplaceClaimAsync(user, positionClaim,
                    new Claim(positionClaim.Type, ProfileViewModel.Position));
            }
            
            this.SuccessMessage = "User Profile updated successfully";
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("User Profile", $"Error occured while updating the user profile : {ex.Message}");
        }
        
        return Page();
    }

    private async Task<(User? user, Claim? departmentClaim, Claim? positionClaim)> GetUserInfoAsync()
    {
        var user = await userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
        if (user != null)
        {
            var claims = await userManager.GetClaimsAsync(user);
            var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
            var positionClaim = claims.FirstOrDefault(x => x.Type == "Position");

            return (user, departmentClaim, positionClaim);
        }
        else
        {
            return (null, null, null);
        }
    }
}

public class UserProfileViewModel
{
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    public string Position { get; set; } = string.Empty;
}