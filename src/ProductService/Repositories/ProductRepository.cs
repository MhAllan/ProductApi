using Microsoft.EntityFrameworkCore;
using ProductService.DomainModels;
using ProductService.Models;
using ProductService.Repositories.Entities;
using System.Runtime.CompilerServices;

namespace ProductService.Repositories;

public class ProductRepository : IProductRepository
{
    readonly DataContext _context;
    public ProductRepository(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddProduct(Product product, CancellationToken ct = default)
    {
        var entity = product.ToEntity();
        _context.Products.Add(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Product> GetProduct(string id, CancellationToken ct = default)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        var result = entity?.ToProduct();

        return result;
    }

    public async IAsyncEnumerable<Product> GetProducts(int skip, int count, 
       [EnumeratorCancellation] CancellationToken ct = default)
    {
        var result = _context.Products.Skip(skip)
                                .Take(count)
                                .AsAsyncEnumerable();

        await foreach (var entity in result)
        {
            yield return entity.ToProduct();
        }
    }
}
