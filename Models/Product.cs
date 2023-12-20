using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_kp.Models;

[Table("products")]
public class Product
{
    [Column("id")] // Specify lowercase column name
    public int ProductId { get; set; }

    [Column("name")] // Specify lowercase column name
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("description")] // Specify lowercase column name
    [StringLength(500)]
    public string? Description { get; set; }

    [Column("price")] // Specify lowercase column name
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public float? Price { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }
    

    [Column("category_id")] // Specify lowercase column name for foreign key
    [Required]
    public int CategoryId { get; set; } // This is the foreign key to the Category table

    
    [ForeignKey("CategoryId")]
    public Category Category { get; set; }
}   