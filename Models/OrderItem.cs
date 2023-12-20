using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_kp.Models;

[Table("order_items")]
public class OrderItem
{
    [Column("order_item_id")]
    public int OrderItemId { get; set; }
    
    [ForeignKey("Product")]
    [Column("product_id")]
    public int ProductId { get; set; }
    public Product Product { get; set; }
    
    [Column("price")]
    [Range(0.01, double.MaxValue, ErrorMessage = "price must be more than 0")]
    public float? Price { get; set; }
    
    [ForeignKey("Order")]
    [Column("order_id")]
    public int OrderId { get; set; }
    public Order Order { get; set; }
    
}