using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppUnderTheHood.Pages;

[Authorize(Policy = "HRManagerOnly")]
public class HRManager : PageModel
{
    public void OnGet()
    {
        
    }
}