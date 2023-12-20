using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_kp.Models;

[Table("orders")]
public class Order
{
    [Column("id")]
    public int OrderId { get; set; }
    
    [ForeignKey("User")]
    [Column("user_id")]
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }
    
    [Column("order_date")]
    [DataType(DataType.DateTime)]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    [Column("order_items")]
    public List<OrderItem> OrderItems { get; set; }
    
}