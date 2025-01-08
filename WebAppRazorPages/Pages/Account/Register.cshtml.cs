using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorPages.Data.Account;
using WebAppRazorPages.Services;

namespace WebAppRazorPages.Pages.Account;

public class Register : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    [BindProperty]
    public RegisterViewModel RegisterViewModel { get; set; } = new();

    public Register(UserManager<User> userManager, IEmailService emailService)
    {
        this._userManager = userManager;
        _emailService = emailService;
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
        var user = new User()
        {
            Email = RegisterViewModel.Email,
            UserName = RegisterViewModel.Email,
            Department = RegisterViewModel.Department,
            Position = RegisterViewModel.Position
        };
        
        var claimDepartment = new Claim("Department", RegisterViewModel.Department);
        var claimPosition = new Claim("Position", RegisterViewModel.Position);

        var result = await _userManager.CreateAsync(user, RegisterViewModel.Password);
        if (result.Succeeded)
        {
            await _userManager.AddClaimAsync(user, claimDepartment);
            await _userManager.AddClaimAsync(user, claimPosition);
            
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var pageLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                values: new { userId = user.Id, token = emailConfirmationToken }) ?? string.Empty;

            await _emailService.SendAsync("kinathru@gmail.com", user.Email, "Please confirm your email",
                $"Please click on this link to confirm your email address : {pageLink}");

            return RedirectToPage("/Account/Login");
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

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    public string Position { get; set; } = string.Empty;
}