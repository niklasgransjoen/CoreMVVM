using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using System;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveLazyTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterTransient<Implementation>().As<IInterface>();
        }

        [Fact]
        public void LifetimeScope_Resolves_Lazy()
        {
            Lazy<IInterface> lazyInstance = LifetimeScope.ResolveRequiredService<Lazy<IInterface>>();

            Assert.False(lazyInstance.IsValueCreated);

            IInterface instance = lazyInstance.Value;

            Assert.IsType<Implementation>(instance);
            Assert.True(lazyInstance.IsValueCreated);
        }

        [Fact]
        public void LifetimeScope_Resolves_UnregistratedLazy()
        {
            Lazy<Class> lazyInstance = LifetimeScope.ResolveRequiredService<Lazy<Class>>();

            Assert.False(lazyInstance.IsValueCreated);

            Class instance = lazyInstance.Value;

            Assert.IsType<Class>(instance);
            Assert.True(lazyInstance.IsValueCreated);
        }

        [Fact]
        public void LifetimeScope_Resolves_UnregistratedLazyWithParameters()
        {
            Lazy<MyClass> lazyInstance = LifetimeScope.ResolveRequiredService<Lazy<MyClass>>();

            Assert.False(lazyInstance.IsValueCreated);

            MyClass instance = lazyInstance.Value;

            Assert.IsType<MyClass>(instance);
            Assert.NotNull(instance.C);
            Assert.True(lazyInstance.IsValueCreated);
        }

        [Fact]
        public void LifetimeScope_Resolves_LazyFunc()
        {
            Lazy<Func<IInterface>> lazyFactory = LifetimeScope.ResolveRequiredService<Lazy<Func<IInterface>>>();

            Assert.False(lazyFactory.IsValueCreated);

            Func<IInterface> factory = lazyFactory.Value;
            Assert.True(lazyFactory.IsValueCreated);

            IInterface instance = factory();
            Assert.NotNull(instance);
        }

        private sealed class MyClass
        {
            public MyClass(Class c)
            {
                C = c;
            }

            public Class C { get; }
        }
    }
}