﻿using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppUnderTheHood.Pages.Account;

public class LogIn : PageModel
{
    [BindProperty]
    public Credential Credential { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Verify the credential
        if (Credential is { Username: "admin", Password: "password" })
        {
            // Create security context
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "admin"),
                new(ClaimTypes.Email, "admin@mywebsite.com"),
                new("Department", "HR"),
                new("Admin", "true"),
                new("Manager", "true"),
                new("EmploymentDate", "2024-05-01"),
            };

            var authenticationType = "MyCookieAuth";
            var identity = new ClaimsIdentity(claims, authenticationType);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = Credential.RememberMe
            };
            await HttpContext.SignInAsync(authenticationType, claimsPrincipal, authenticationProperties);
            
            return RedirectToPage("/Index");
        }
        
        return Page();
    }
}

public class Credential
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