using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace dotnet_kp.Pages.Admin.OrderItems;

public class OrderItemInputModel
{
    public int ProductId { get; set; }
    public float Price { get; set; }
    public int OrderId { get; set; }
    public int UserId { get; set; }
}

[Authorize(policy: "AdminOnly")]
public class OrderItemModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OrderItemModel> _logger;

    public OrderItemModel(ApplicationDbContext dbContext, ILogger<OrderItemModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public List<OrderItem> AllOrderItems { get; set; }
    public List<Product> AllProducts { get; set; }
    public List<Order> AllOrders { get; set; }
    public List<User> AllUsers { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? OrderItemId { get; set; }
    [BindProperty] public OrderItemInputModel OrderItemInput { get; set; }

    public void OnGet()
    {
        AllOrderItems = _dbContext.OrderItems.Include(i => i.Product)
            .Include(i => i.Order).ToList();
        AllProducts = _dbContext.Products.ToList();
        AllOrders = _dbContext.Orders.ToList();
        AllUsers = _dbContext.Users.ToList();
    }

    public async Task<IActionResult> OnPostRemoveOrderItemAsync(int orderItemId)
    {
        _logger.LogInformation("i am at remove orderItem");
        try
        {
            var OrderItemToDelete = _dbContext.OrderItems.FirstOrDefault(c => c.OrderItemId == orderItemId);
            if (OrderItemToDelete == null)
            {
                return NotFound();
            }

            _dbContext.OrderItems.Remove(OrderItemToDelete);
            var ordersWithMatchingOrderItem = _dbContext.Orders
                .Where(o => o.OrderItems.Any(oi => oi.OrderItemId == orderItemId))
                .ToList();

            foreach (var order in ordersWithMatchingOrderItem)
            {
                _dbContext.Orders.Remove(order);
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Admin/OrderItems/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        _logger.LogInformation("i am at post orderItems");
        try
        {
            var order = new Order
            {
                UserId = OrderItemInput.UserId
            };
            var newOrderItem = new OrderItem
            {
                ProductId = OrderItemInput.ProductId,
                Price = OrderItemInput.Price,
                OrderId = order.OrderId
            };
            order.OrderItems = new List<OrderItem> { newOrderItem };
            order.OrderItems.Add(newOrderItem);
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();
            return RedirectToPage("/Admin/OrderItems/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        return Page();
    }
}