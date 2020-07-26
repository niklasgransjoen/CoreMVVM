using CoreMVVM.IOC.Builder;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolvesGenericsTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterLifetimeScope(typeof(Service<>)).As(typeof(IService<>));
        }

        [Fact]
        public void LifetimeScope_Resolves_Generics()
        {
            var service1 = LifetimeScope.ResolveRequiredService<IService<int>>();
            var service2 = LifetimeScope.ResolveRequiredService<IService<string>>();

            Assert.IsType<Service<int>>(service1);
            Assert.IsType<Service<string>>(service2);
        }

        private interface IService<T>
        {
        }

        private class Service<T> : IService<T>
        {
        }
    }
}