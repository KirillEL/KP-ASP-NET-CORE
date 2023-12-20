using System.Security.Claims;
using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace dotnet_kp.Pages.Cart;

public class CartModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CartModel> _logger;

    public CartModel(ApplicationDbContext dbContext, ILogger<CartModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public List<Order> Orders { get; set; } = new List<Order>();


    public IActionResult OnGet()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Index");
        }

        var userIdClaim = User.FindFirst("userId");

        if (userIdClaim != null)
        {
            var userId = userIdClaim.Value;
            Orders = _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId.ToString() == userId)
                .ToList();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteProductFromCartAsync(int orderId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Index");
        }

        var userIdClaim = User.FindFirst("userId");

        if (userIdClaim != null)
        {
            var userId = userIdClaim.Value;

            var order = _dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId.ToString() == userId);

            if (order != null)
            {
                // Find the specific product to delete (if it exists) or remove the entire order.
                var orderItem = order.OrderItems.FirstOrDefault();

                if (orderItem != null)
                {
                    order.OrderItems.Remove(orderItem);
                }

                // Check if there are no more items in the order
                if (order.OrderItems.Count == 0)
                {
                    _dbContext.Orders.Remove(order);
                    Orders.Remove(order); // Remove it from the list as well
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        return RedirectToPage("/Cart/Index");
    }


    public JsonResult OnGetOrdersCount()
    {
        var userIdClaim = User.FindFirst("userId");
        int count = 0; // Initialize the count to 0.

        if (userIdClaim != null)
        {
            var userId = userIdClaim.Value;
            count = _dbContext.Orders
                .Where(o => o.UserId.ToString() == userId)
                .Include(o => o.OrderItems)
                .Sum(o => o.OrderItems.Count);
        }

        return new JsonResult(new { count });
    }
}