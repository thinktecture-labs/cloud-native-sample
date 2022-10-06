using System.ComponentModel.DataAnnotations;

namespace PriceWatcher.Models;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    public double Price { get; set; }
}
