using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppUnderTheHood.Pages;

[Authorize(Policy = "AdminOnly")]
public class Settings : PageModel
{
    public void OnGet()
    {
        
    }
}