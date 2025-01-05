using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppUnderTheHood.Pages;

[Authorize(Policy = "MustBelongToHRDepartment")]
public class HumanResources : PageModel
{
    public void OnGet()
    {
        
    }
}