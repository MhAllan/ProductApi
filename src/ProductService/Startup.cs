using Microsoft.AspNetCore.Mvc;
using ProductService.Middleware;
using ProductService.Controllers;
using ProductService.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using ProductService.Repositories;
using ProductService.ActionFilters;

namespace ProductService;

public class Startup
{
    readonly IConfiguration _config;
    readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration config, IWebHostEnvironment environment)
    {
        _config = config;
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();


        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressMapClientErrors = true;
        });

        services.AddOptions<ProductControllerConfig>()
            .Bind(_config.GetSection("ProductControllerConfig"))
            .ValidateDataAnnotations();

        services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("ProductDb"));

        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddControllers(options =>
        {
            options.EnableEndpointRouting = false;
            options.Filters.Add(new ModelValidationFilter());

        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseMiddleware<ErrorHandlerMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}