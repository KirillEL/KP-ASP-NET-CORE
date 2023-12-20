using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace dotnet_kp.Pages.Admin.Orders;

[Authorize(policy: "AdminOnly")]
public class OrdersModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OrdersModel> _logger;

    public OrdersModel(ApplicationDbContext dbContext, ILogger<OrdersModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public List<Order> AllOrders { get; set; }
    public void OnGet()
    {
        AllOrders = _dbContext.Orders.Include(io => io.User)
            .Include(io => io.OrderItems).ThenInclude(oi => oi.Product).ToList();
    }
}