using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_kp.Models;

[Table("users")]
public class User
{
    [Column("id")]
    public int UserId { get; set; }

    [Column("username")]
    [Required]
    [StringLength(50)]
    public string UserName { get; set; }

    [Column("email")]
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [Column("password")]
    [Required]
    [MinLength(6, ErrorMessage = "password must me at least 6 characters long")]
    public string Password { get; set; }


    [Column("role")]
    [DefaultValue("user")]
    public string Role { get; set; } = "user";

}