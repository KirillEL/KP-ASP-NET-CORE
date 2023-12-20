using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_kp.Pages.Login;

public class UserLogin
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}

public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ApplicationDbContext dbContext, IConfiguration iconfiguration, ILogger<LoginModel> logger)
    {
        _dbContext = dbContext;
        _configuration = iconfiguration;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    [BindProperty] 
    public UserLogin UserLogin { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        
        if (ModelState.IsValid)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == UserLogin.Email);

            if (user != null && (UserLogin.Password == user.Password))
            {
                var token = GenerateJwtToken(user);
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("userId", user.UserId.ToString()),
                    new Claim("user_role", user.Role.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToPage("/Index");
            }
            else
            {
                ViewData["ErrorMessage"] = "Invalid email or password.";
            }
            
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return Page();
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("userId", user.UserId.ToString()),
            new Claim("user_role", user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(30), 
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}