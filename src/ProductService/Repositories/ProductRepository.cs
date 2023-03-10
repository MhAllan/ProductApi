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
        var product = await _context.Products.Select(x => new Product
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                Price = x.Price
                            })
                            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return product;
    }

    public IAsyncEnumerable<Product> GetProducts(int skip, int count, CancellationToken ct = default)
    {
        var result = _context.Products.Skip(skip)
                                .Take(count)
                                .Select(p => new Product
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    Description = p.Description,
                                    Price = p.Price,
                                })
                                .AsAsyncEnumerable();

        return result;
    }
}
