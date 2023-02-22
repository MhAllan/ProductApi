using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ProductService.Tests;

public class AppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    static readonly Type _startupType;
    static readonly Assembly _startupAccembly;
    static AppFactory()
    {
        _startupType = typeof(TStartup);
        _startupAccembly = _startupType.Assembly;
    }

    Action<IServiceCollection> _testServicesConfigurator;
    Dictionary<string, string> _testConfigurations = new Dictionary<string, string>();

    protected override IWebHostBuilder CreateWebHostBuilder()
    {
        return WebHost.CreateDefaultBuilder()
                       .UseEnvironment("Development")
                       .UseStartup<TStartup>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddMvc().AddApplicationPart(_startupAccembly);
        });

        if (_testServicesConfigurator != null)
        {
            builder.ConfigureTestServices(_testServicesConfigurator);
        }

        builder.ConfigureAppConfiguration((context, builder) =>
        {
            builder.AddInMemoryCollection(_testConfigurations);
        });
    }

    public void ConfigureTestServices(Action<IServiceCollection> options)
    {
        _testServicesConfigurator = options;
    }

    public void SetConfiguration(string key, string value)
    {
        _testConfigurations.Add(key, value);
    }

    public bool TryGetService<T>(out T service)
    {
        service = default;
        var obj = Services.GetService(typeof(T));
        if (obj == null)
            return false;

        service = (T)obj;
        return true;
    }

    public T GetService<T>()
    {
        if (!TryGetService<T>(out var result))
            throw new Exception($"could not resolve service of type {typeof(T)}");

        return result;
    }

    public IConfiguration Configuration => GetService<IConfiguration>();
    public T GetConfig<T>(string section) => Configuration.GetSection(section).Get<T>();
    public IOptions<T> GetOptions<T>() where T : class, new() => GetService<IOptions<T>>();
}
