using CoreMVVM.IOC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace CoreMVVM.Extensions.DependencyInjection
{
    /// <summary>
    /// Implementation for using <see cref="IServiceScopeFactory"/>. with CoreMVVM's container.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <code>
    /// var builder = new ContainerBuilder();<br/>
    /// ...<br/>
    /// builder.RegisterSingleton&lt;CoreMVVMServiceScopeFactory&gt;().As&lt;IServiceScopeFactory&gt;();<br/>
    /// ...<br/>
    /// // Get a service provider that supports scopes.<br/>
    /// var serviceProvider = builder.Build()<br/>
    ///     .ResolveRequiredService&lt;IServiceScopeFactory&gt;()<br/>
    ///     .ServiceProvider;
    /// </code>
    /// </remarks>
    public sealed class CoreMVVMServiceScopeFactory : IServiceScopeFactory
    {
        private readonly ScopeableILifetimeScope _scopeableILifetimeScope;

        public CoreMVVMServiceScopeFactory(IContainer container)
        {
            _scopeableILifetimeScope = new ScopeableILifetimeScope(container);
        }

        public IServiceProvider ServiceProvider => _scopeableILifetimeScope;

        public IServiceScope CreateScope()
        {
            return new ServiceScope(_scopeableILifetimeScope);
        }

        private sealed class ServiceScope : IServiceScope
        {
            private readonly ScopeableILifetimeScope _scopeableILifetimeScope;
            private bool _isDisposed;

            public ServiceScope(ScopeableILifetimeScope scopeableILifetimeScope)
            {
                scopeableILifetimeScope.BeginLifetimeScope();
                _scopeableILifetimeScope = scopeableILifetimeScope;
            }

            public IServiceProvider ServiceProvider => _scopeableILifetimeScope;

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
                _scopeableILifetimeScope.Dispose();
            }
        }

        /// <summary>
        /// Wrapper for <see cref="ILifetimeScope"/> to support <see cref="IServiceScope"/>.
        /// </summary>
        private sealed class ScopeableILifetimeScope : ILifetimeScope
        {
            private readonly Stack<ILifetimeScope> _scopes = new Stack<ILifetimeScope>();

            public ScopeableILifetimeScope(ILifetimeScope initialScope)
            {
                _scopes.Push(initialScope);
            }

            private ILifetimeScope CurrentScope => _scopes.Peek();
            public bool IsDisposed => CurrentScope.IsDisposed;

            public ILifetimeScope BeginLifetimeScope()
            {
                var subScope = CurrentScope.BeginLifetimeScope();
                _scopes.Push(subScope);

                return this;
            }

            public void Dispose()
            {
                CurrentScope.Dispose();
                if (_scopes.Count > 1)
                    _scopes.Pop();
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IServiceProvider))
                    return this;

                return CurrentScope.GetService(serviceType);
            }
        }
    }
}