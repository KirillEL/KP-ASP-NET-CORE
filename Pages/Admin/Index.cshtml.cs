using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotnet_kp.Pages.Admin;
[Authorize(policy: "AdminOnly")]
public class AdminModel : PageModel
{
    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        Response.Cookies.Delete("jwtToken");
        return RedirectToPage("/Index");
    }
}