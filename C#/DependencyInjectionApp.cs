
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public interface IService { void Serve(); }
public class ServiceA : IService { public void Serve() => Console.WriteLine("Service A is serving."); }
public class ServiceB : IService { public void Serve() => Console.WriteLine("Service B is serving."); }

public class Consumer
{
    private readonly IService _service;

    // Constructor injection
    public Consumer(IService service) { _service = service; }

    // Property injection
    public IService AnotherService { get; set; }

    public void Start()
    {
        _service.Serve();
        AnotherService?.Serve();
    }
}

public class DependencyContainer
{
    private readonly Dictionary<Type, Type> _services = new Dictionary<Type, Type>();

    public void Register<TService, TImplementation>() where TImplementation : TService
    {
        _services[typeof(TService)] = typeof(TImplementation);
    }

    public T Resolve<T>()
    {
        return (T)Resolve(typeof(T));
    }

    private object Resolve(Type serviceType)
    {
        if (!_services.ContainsKey(serviceType)) throw new Exception($"Service of type {serviceType.Name} is not registered.");

        Type implementationType = _services[serviceType];
        ConstructorInfo constructor = implementationType.GetConstructors().First();

        var parameters = constructor.GetParameters().Select(param => Resolve(param.ParameterType)).ToArray();
        var instance = Activator.CreateInstance(implementationType, parameters);

        // Property injection
        foreach (var property in implementationType.GetProperties().Where(prop => prop.CanWrite && _services.ContainsKey(prop.PropertyType)))
        {
            property.SetValue(instance, Resolve(property.PropertyType));
        }

        return instance;
    }
}

public class Program
{
    public static void Main()
    {
        var container = new DependencyContainer();

        // Register services
        container.Register<IService, ServiceA>();
        container.Register<Consumer, Consumer>();

        // Resolve and use Consumer with dependency injection
        var consumer = container.Resolve<Consumer>();
        consumer.AnotherService = new ServiceB(); // Inject another service using property injection
        consumer.Start();
    }
}
