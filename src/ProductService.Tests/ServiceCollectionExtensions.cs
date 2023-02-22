using Microsoft.Extensions.DependencyInjection;

namespace ProductService.Tests;

public static class ServiceCollectionExtensions
{
    public static void Replace<TInterface, TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime, bool ignoreUnfoundService = false)
        where TInterface : class
        where TImplementation : class, TInterface

    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TInterface));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
        else if (!ignoreUnfoundService)
        {
            throw new Exception($"There is no service registered for type {typeof(TInterface).FullName}");
        }

        services.Add(new ServiceDescriptor(typeof(TInterface), typeof(TImplementation), lifetime));
    }

    //if we use T as parameter like this: ReplaceSingleton<T>(this IServiceCollection services, T instance)
    //The compiler will infer T from the instance, and will prompt that you can remove T when you call the method
    //And we don't want that because T is the interface to replace
    //Also I don't like ReplaceSingleton<Tintf, T> because that will need to alaways pass the generic parameters
    //which makes the unit test more bulky
    public static void ReplaceSingleton<T>(this IServiceCollection services, object instance,
        bool ignoreUnfoundService = false) where T : class
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance));

        if (!typeof(T).IsAssignableFrom(instance.GetType()))
        {
            throw new Exception($"Type {instance.GetType()} does not implement interface {typeof(T)}");
        }

        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
        else if (!ignoreUnfoundService)
        {
            throw new Exception($"There is no service registered for type {typeof(T).FullName}");
        }
        services.Add(new ServiceDescriptor(typeof(T), instance));
    }
}
