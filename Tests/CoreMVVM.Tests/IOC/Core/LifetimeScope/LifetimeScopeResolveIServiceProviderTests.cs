using System;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveIServiceProviderTests : LifetimeScopeTestBase
    {
        [Fact]
        public void LifetimeScope_Resolves_IContainer()
        {
            var ex = Record.Exception(() => LifetimeScope.ResolveRequiredService<IContainer>());
            Assert.Null(ex);
        }

        [Fact]
        public void LifetimeScope_Resolves_IContainer_ToSameInstance()
        {
            IContainer c1 = LifetimeScope.ResolveRequiredService<IContainer>();
            IContainer c2 = LifetimeScope.ResolveRequiredService<IContainer>();

            Assert.Equal(c1, c2);
        }

        [Fact]
        public void LifetimeScope_Resolves_ILifetimeScope()
        {
            ILifetimeScope res1 = LifetimeScope.ResolveRequiredService<ILifetimeScope>();
            Assert.Same(LifetimeScope, res1);

            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();
            ILifetimeScope res2 = subscope.ResolveRequiredService<ILifetimeScope>();
            Assert.Same(subscope, res2);
        }

        [Fact]
        public void LifetimeScope_Resolves_IServiceProvider()
        {
            IServiceProvider res1 = LifetimeScope.ResolveRequiredService<IServiceProvider>();
            Assert.Same(LifetimeScope, res1);

            IServiceProvider subscope = LifetimeScope.BeginLifetimeScope();
            IServiceProvider res2 = subscope.ResolveRequiredService<IServiceProvider>();
            Assert.Same(subscope, res2);
        }
    }
}