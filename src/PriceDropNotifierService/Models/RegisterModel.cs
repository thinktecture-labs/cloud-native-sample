using System.ComponentModel.DataAnnotations;

namespace PriceDropNotifier.Models;

public sealed class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public Guid ProductId { get; set; }
    
    [Range(0.0, double.MaxValue)]
    public double Price { get; set; }
}
