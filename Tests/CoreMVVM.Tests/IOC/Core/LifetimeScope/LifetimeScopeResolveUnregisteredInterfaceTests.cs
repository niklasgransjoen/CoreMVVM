using CoreMVVM.IOC.Builder;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public sealed class LifetimeScopeResolveUnregisteredInterfaceTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
        }

        [Fact]
        public void LifetimeScope_Resolves_Unregistered_Logger()
        {
            var logger = LifetimeScope.ResolveRequiredService<IService>();
            Assert.IsType<Service>(logger);
        }

        #region Cache

        [Fact]
        public void LifetimeScope_Fallback_Caches()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSingleton<ResolveLoggerService>().As<IResolveUnregisteredInterfaceService>();
            var container = builder.Build();

            var service = (ResolveLoggerService)container.ResolveRequiredService<IResolveUnregisteredInterfaceService>();
            service.Cache = true;

            var logger1 = container.ResolveRequiredService<IService>();
            var logger2 = container.ResolveRequiredService<IService>();

            // Assert that resolve service was only invoked once.
            Assert.Equal(1, service.CallCount);
            Assert.Same(logger1, logger2);
        }

        #endregion Cache

        #region Don't cache

        [Fact]
        public void LifetimeScope_Fallback_DoesNot_Cache()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSingleton<ResolveLoggerService>().As<IResolveUnregisteredInterfaceService>();
            var container = builder.Build();

            var service = (ResolveLoggerService)container.ResolveRequiredService<IResolveUnregisteredInterfaceService>();
            service.Cache = false;

            container.ResolveRequiredService<IService>();
            container.ResolveRequiredService<IService>();

            // Assert that resolve service was actually invoked twice.
            Assert.Equal(2, service.CallCount);
        }

        #endregion Don't cache

        private sealed class ResolveLoggerService : IResolveUnregisteredInterfaceService
        {
            public int CallCount { get; private set; }
            public bool Cache { get; set; }

            public void Handle(ResolveUnregisteredInterfaceContext context)
            {
                CallCount++;
                context.SetInterfaceImplementationType(typeof(Service));
                context.CacheImplementation = Cache;
                context.CacheScope = ComponentScope.Singleton;
            }
        }

        [FallbackImplementation(typeof(Service))]
        private interface IService
        {
        }

        private sealed class Service : IService
        {
        }
    }
}