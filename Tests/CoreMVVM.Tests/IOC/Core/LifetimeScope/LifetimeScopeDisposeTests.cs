using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeDisposeTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterTransient<Disposable>().As<IDisposableInterface>();
            builder.RegisterSingleton<DisposableSingleton>().AsSelf();
            builder.RegisterLifetimeScope<DisposableLifetimeScopedResource>().AsSelf();
        }

        [Fact]
        public void LifetimeScope_Disposes_IDisposables()
        {
            var instance = LifetimeScope.ResolveRequiredService<IDisposableInterface>();

            Assert.False(instance.IsDisposed);
            LifetimeScope.Dispose();

            Assert.True(instance.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_Only_Disposes_Children()
        {
            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();

            var instance = LifetimeScope.ResolveRequiredService<Disposable>();
            var subInstance = subscope.ResolveRequiredService<Disposable>();

            Assert.False(subInstance.IsDisposed);
            subscope.Dispose();

            Assert.True(subInstance.IsDisposed);
            Assert.False(instance.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_Disposes_SubScopes()
        {
            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();

            Assert.False(LifetimeScope.IsDisposed);
            Assert.False(subscope.IsDisposed);

            LifetimeScope.Dispose();

            Assert.True(LifetimeScope.IsDisposed);
            Assert.True(subscope.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_Disposes_Singleton()
        {
            var disposable = LifetimeScope.ResolveRequiredService<DisposableSingleton>();

            Assert.False(disposable.IsDisposed);
            LifetimeScope.Dispose();

            Assert.True(disposable.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_Disposes_LifetimeScope()
        {
            var disposable = LifetimeScope.ResolveRequiredService<DisposableLifetimeScopedResource>();

            Assert.False(disposable.IsDisposed);
            LifetimeScope.Dispose();

            Assert.True(disposable.IsDisposed);
        }
    }
}