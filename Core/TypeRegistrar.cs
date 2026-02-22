using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace ConBot.Core;

public sealed class TypeRegistrar(IServiceCollection builder) : ITypeRegistrar
{
    public ITypeResolver Build() => new TypeResolver(builder.BuildServiceProvider());

    public void Register(Type service, Type implementation) => builder.AddSingleton(service, implementation);
    public void RegisterInstance(Type service, object implementation) => builder.AddSingleton(service, implementation);
    public void RegisterLazy(Type service, Func<object> factory) => builder.AddSingleton(service, _ => factory());

    private sealed class TypeResolver(IServiceProvider provider) : ITypeResolver, IDisposable
    {
        public object? Resolve(Type? type) => type != null ? provider.GetService(type) : null;
        public void Dispose() => (provider as IDisposable)?.Dispose();
    }
}