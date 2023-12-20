using System.Security.Claims;
using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace dotnet_kp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ApplicationDbContext _context;

    public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _context = dbContext;
    }

    public List<Product> Products { get; set; }
    public List<Category> Categories { get; set; }

    public void OnGet()
    {
        Products = _context.Products.ToList();
        Categories = _context.Categories.ToList();
    }

    public IActionResult OnPostSignOut()
    {
        Response.Cookies.Delete("jwtToken");
        return RedirectToPage("/Index");
    }


    public void OnPost(string category)
    {
        if (category == "all")
        {
            Products = _context.Products.ToList();
            Categories = _context.Categories.ToList();
        }
        else
        {
            Products = _context.Products
                .Join(_context.Categories, p => p.CategoryId, c => c.CategoryId,
                    (product, category) => new { Product = product, Category = category })
                .Where(joinedData => joinedData.Category.Name == category)
                .Select(joinedData => joinedData.Product)
                .ToList();
            Categories = _context.Categories.ToList();
        }
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId)
    {
        if (User.Identity.IsAuthenticated)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;

                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);

                var order = _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.UserId == Convert.ToInt32(userId) && o.OrderItems.Any(oi => oi.ProductId == productId));

                if (order == null)
                {
                    order = new Order
                    {
                        UserId = Convert.ToInt32(userId),
                        OrderItems = new List<OrderItem>
                            { new OrderItem { ProductId = productId, Price = product.Price } }
                    };
                    _context.Orders.Add(order);
                }
                else
                {
                    order.OrderItems.Add(new OrderItem { ProductId = productId, Price = product.Price });
                }

                await _context.SaveChangesAsync();

                
            }
        }
        else
        {

            return StatusCode(403);
        }

        return Page();
    }
}