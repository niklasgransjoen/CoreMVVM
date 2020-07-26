using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveTransientTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterTransient<Implementation>().As<IInterface>();
        }

        [Fact]
        public void LifetimeScope_ResolvesInterface_ToRegistration()
        {
            IInterface subject = LifetimeScope.ResolveRequiredService<IInterface>();

            Assert.IsType<Implementation>(subject);
        }

        [Fact]
        public void LifetimeScope_ResolvesRegistrations_ToUniqueInstances()
        {
            object subject1 = LifetimeScope.ResolveRequiredService<IInterface>();
            object subject2 = LifetimeScope.ResolveRequiredService<IInterface>();

            Assert.NotSame(subject1, subject2);
        }
    }
}