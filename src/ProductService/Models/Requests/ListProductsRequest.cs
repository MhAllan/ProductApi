using System.ComponentModel.DataAnnotations;

namespace ProductService.Models.Requests;

public class ListProductsRequest
{
    [Range(1, int.MaxValue)]
    public int? Page { get; set; }
}
