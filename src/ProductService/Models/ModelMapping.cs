using ProductService.DomainModels;
using ProductService.Models.Requests;
using ProductService.Repositories.Entities;
namespace ProductService.Models;

public static class ModelMapping
{
    public static Product ToProduct(this CreateProductRequest request)
    {
        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };
    }

    public static ProductEntity ToEntity(this Product product)
    {
        return new ProductEntity
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
    }
}
