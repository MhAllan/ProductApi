using System.ComponentModel.DataAnnotations;

namespace ProductService.Models.Requests;

public class CreateProductRequest : IValidatableObject
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; }

    [Required]
    [StringLength(500)]
    public string Description { get; init; }

    [Required]
    [Range(0.0000001, double.MaxValue)]
    public decimal Price { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //add complex validation here

        return Enumerable.Empty<ValidationResult>();
    }
}
