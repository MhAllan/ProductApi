using System.ComponentModel.DataAnnotations;

namespace ProductService.Controllers;

public class ProductControllerConfig
{
    [Range(1, 100)]
    public int ListPageSize { get; init; }
}
