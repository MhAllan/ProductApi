using FluentAssertions;
using Moq;
using ProductService.DomainModels;
using ProductService.Models.Requests;
using ProductService.Repositories;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductService.Tests.Controllers;

public class ProductControllerTest : EndToEndTest<Startup>
{
    Product _anyProd => It.IsAny<Product>();
    CancellationToken _anyCt => It.IsAny<CancellationToken>();
    string _anyStr => It.IsAny<string>();
    int _anyInt => It.IsAny<int>();

    [Fact]
    public async Task CreateProduct_Success()
    {
        Product producedProd = null;
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(x => x.AddProduct(_anyProd, _anyCt))
                .Callback((Product prod, CancellationToken ct) =>
                {
                    producedProd = prod;
                });

        var request = new CreateProductRequest
        {
            Name = "name-01",
            Description = "desc",
            Price = 0.14m
        };

        var expected = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = 0.14m
        };

        AppFactory.ConfigureTestServices(services =>
        {
            services.ReplaceSingleton<IProductRepository>(repoMock.Object);
        });

        var client = AppFactory.CreateClient();
        var response = await client.PostAsJsonAsync("product", request);

        Assert.True(response.IsSuccessStatusCode);

        var result = await response.Content.ReadFromJsonAsync<Product>();
        Assert.False(string.IsNullOrEmpty(result.Id));

        expected.Should().BeEquivalentTo(result, opt => opt.Excluding(x => x.Id));

        //The rest is usually not necessary, but if something too sensitive and team have time then can do:
        Assert.NotNull(producedProd);
        repoMock.Verify(x => x.AddProduct(producedProd, _anyCt), Times.Once());
        expected.Should().BeEquivalentTo(producedProd, opt => opt.Excluding(x => x.Id));
        //end of unnecessary code
    }

    [Fact]
    public async Task CreateProduct_Success_NoMocks()
    {
        var request = new CreateProductRequest
        {
            Name = "name-01",
            Description = "desc",
            Price = 0.14m
        };

        var expected = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = 0.14m
        };

        var client = AppFactory.CreateClient();
        var response = await client.PostAsJsonAsync("product", request);

        Assert.True(response.IsSuccessStatusCode);

        var result = await response.Content.ReadFromJsonAsync<Product>();
        Assert.False(string.IsNullOrEmpty(result.Id));

        expected.Should().BeEquivalentTo(result, opt => opt.Excluding(x => x.Id));
    }


    public static IEnumerable<object[]> CreateProduct_BadRequest_Data => new[]
    {
        //missing name
        new[] { new CreateProductRequest {  Description = "desc", Price = 0.1m } },

        //price out of range
        new[] { new CreateProductRequest { Name = "name-01", Description = "desc", Price = -0.1m } }

        //and so on, I will not do more for the time
    };

    [Theory]
    [MemberData(nameof(CreateProduct_BadRequest_Data))]
    public async Task CreateProduct_BadRequest(CreateProductRequest request)
    {
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(x => x.AddProduct(_anyProd, _anyCt));

        AppFactory.ConfigureTestServices(services =>
        {
            services.ReplaceSingleton<IProductRepository>(repoMock.Object);
        });

        var client = AppFactory.CreateClient();
        var response = await client.PostAsJsonAsync("product", request);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task GetProduct_NoProduct_Should_Return_NotFound()
    {
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(x => x.GetProduct(_anyStr, _anyCt))
                .ReturnsAsync((Product)null);

        AppFactory.ConfigureTestServices(services =>
        {
            services.ReplaceSingleton<IProductRepository>(repoMock.Object);
        });

        var client = AppFactory.CreateClient();
        var response = await client.GetAsync("product/1");

        Assert.True(response.StatusCode == HttpStatusCode.NotFound);
    }

    //I will drop test here as it really takes time
    //And so on for all endpoints, different status codes, and different input scenarios
}
