using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using System;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveFuncTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterTransient<Implementation>().As<IInterface>();
            builder.RegisterTransient(c => (Class)null).AsSelf();
        }

        [Fact]
        public void LifetimeScope_Resolves_Func()
        {
            Func<IInterface> factory = LifetimeScope.ResolveRequiredService<Func<IInterface>>();

            Assert.NotNull(factory);
            Assert.IsType<Func<IInterface>>(factory);

            object instance = factory();
            Assert.IsType<Implementation>(instance);
        }

        [Fact]
        public void LifetimeScope_Handles_NullReturningFunc()
        {
            Assert.Throws<ResolveException>(() => LifetimeScope.ResolveRequiredService<Class>());
        }
    }
}