using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Xunit.Abstractions;
using Xunit;

namespace ProductService.Tests;

public abstract class EndToEndTest<TStartup> : IClassFixture<AppFactory<TStartup>> where TStartup : class
{
    protected readonly AppFactory<TStartup> AppFactory;

    //use this for same shared factory
    public EndToEndTest(AppFactory<TStartup> factory)
    {
        AppFactory = factory;
    }

    public EndToEndTest() : this(new AppFactory<TStartup>())
    {
    }
}