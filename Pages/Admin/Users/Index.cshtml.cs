using System.ComponentModel.DataAnnotations;
using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dotnet_kp.Pages.Admin.Users;

public class UserInputModel
{
    [StringLength(50)]
    public string? UserName { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    public string? Password { get; set; }

    public string? Role { get; set; } = "user";
}

[Authorize(policy: "AdminOnly")]
public class UserModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UserModel> _logger;

    public UserModel(ApplicationDbContext dbContext, ILogger<UserModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public List<User> AllUsers { get; set; }
    [BindProperty(SupportsGet = true)]
    public int? UserId { get; set; }
    
    
    [BindProperty] 
    public UserInputModel UserInput { get; set; }
    
    public void OnGet()
    {
        AllUsers = _dbContext.Users.ToList();
    }

    public async Task<IActionResult> OnPostRemoveUserAsync(int userId)
    {
        try
        {
            var userFind = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (userFind == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(userFind);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Admin/Users/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return Page();
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        _logger.LogInformation("add user");
        if (ModelState.IsValid)
        {
            try
            {
                var user = new User
                {
                    UserName = UserInput.UserName,
                    Email = UserInput.Email,
                    Password = UserInput.Password,
                    Role = UserInput.Role
                };
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return RedirectToPage("/Admin/Users/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        return Page();
    }


    public async Task<IActionResult> OnPostEditUserAsync(int userId)
    {
        _logger.LogInformation("i am in edit user");
        try
        {
            var userToChange = _dbContext.Users.FirstOrDefault(c => c.UserId == userId);
            if (userToChange != null)
            {
                userToChange.UserName = string.IsNullOrEmpty(UserInput.UserName) ? userToChange.UserName : UserInput.UserName;
                userToChange.Email = string.IsNullOrEmpty(UserInput.Email) ? userToChange.Email : UserInput.Email;
                userToChange.Password = string.IsNullOrEmpty(UserInput.Password) ? userToChange.Password : UserInput.Password;
                userToChange.Role = string.IsNullOrEmpty(UserInput.Role) ? userToChange.Role : UserInput.Role;
                await _dbContext.SaveChangesAsync();
                return RedirectToPage("/Admin/Users/Index");
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        return Page();
    }
    
}