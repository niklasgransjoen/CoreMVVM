using CoreMVVM.IOC.Builder;
using System;

namespace CoreMVVM.IOC.Core.Tests
{
    public abstract class LifetimeScopeTestBase : IDisposable
    {
        protected LifetimeScopeTestBase()
        {
            ContainerBuilder builder = new ContainerBuilder();
            RegisterComponents(builder);

            IContainer container = builder.Build();

            LifetimeScope = container;
        }

        protected ILifetimeScope LifetimeScope { get; }

        protected virtual void RegisterComponents(ContainerBuilder builder)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            LifetimeScope.Dispose();
        }
    }
}