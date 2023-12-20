using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dotnet_kp.Pages.Register;

public class RegisterModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;

    public RegisterModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [BindProperty]
    public User User { get; set; }
    
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine(User);
        if (ModelState.IsValid)
        {
            if (_dbContext.Users.Any(u => u.Email == User.Email))
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким email уже зарегистрирован.");
                return Page();
            }
        }
        _dbContext.Users.Add(User);
        await _dbContext.SaveChangesAsync();

        return RedirectToPage("/Login/Index");
    }
}