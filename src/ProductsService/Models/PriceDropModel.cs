using System.ComponentModel.DataAnnotations;

namespace ProductsService.Models;

// I'd actually call this PriceDropDto, but went with the existing code style
public sealed class PriceDropModel
{
    public Guid ProductId { get; set; }
    
    [Range(0.0, 1.0, ErrorMessage = "You must specify a percentage value between 0.0 and 1.0")]
    public double DropPriceBy { get; set; }
}
