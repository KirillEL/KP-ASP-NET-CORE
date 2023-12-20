using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_kp.Models;

[Table("categories")]
public class Category
{
    [Column("id")]
    public int CategoryId { get; set; }

    [Column("name")]
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Column("description")]
    [StringLength(200)]
    public string Description { get; set; }
}