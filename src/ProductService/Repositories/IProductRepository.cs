using ProductService.DomainModels;

namespace ProductService.Repositories;

public interface IProductRepository
{
    Task AddProduct(Product product, CancellationToken ct = default);
    Task<Product> GetProduct(string id, CancellationToken ct = default);
    IAsyncEnumerable<Product> GetProducts(int skip, int count, CancellationToken ct = default);
}
