using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using System;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveOwnedTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterTransient<Implementation>().As<IInterface>();

            builder.RegisterTransient<Disposable>().As<IDisposableInterface>();
            builder.RegisterSingleton<DisposableSingleton>().AsSelf();
            builder.RegisterLifetimeScope<DisposableLifetimeScopedResource>().AsSelf();
        }

        [Fact]
        public void LifetimeScope_Resolves_Owned()
        {
            Owned<IInterface> ownedInstance = LifetimeScope.ResolveRequiredService<Owned<IInterface>>();

            Assert.NotNull(ownedInstance);
            Assert.NotNull(ownedInstance.Value);
        }

        [Fact]
        public void LifetimeScope_Resolves_IOwned()
        {
            IOwned<IInterface> ownedInstance = LifetimeScope.ResolveRequiredService<IOwned<IInterface>>();

            Assert.NotNull(ownedInstance);
            Assert.NotNull(ownedInstance.Value);
        }

        [Fact]
        public void LifetimeScope_DoesNotDisposed_OwnedComponent()
        {
            Owned<IDisposableInterface> disposable = LifetimeScope.ResolveRequiredService<Owned<IDisposableInterface>>();

            Assert.False(disposable.Value.IsDisposed);

            LifetimeScope.Dispose();
            Assert.False(disposable.Value.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_DoesNotOwn_FuncResult()
        {
            Func<IDisposableInterface> factory = LifetimeScope.ResolveRequiredService<Func<IDisposableInterface>>();
            IDisposableInterface instance = factory();

            Assert.False(instance.IsDisposed);
            LifetimeScope.Dispose();

            Assert.True(instance.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_Owns_FuncResult()
        {
            var factory = LifetimeScope.ResolveRequiredService<IOwned<Func<IDisposableInterface>>>();
            IDisposableInterface instance = factory.Value();

            Assert.False(instance.IsDisposed);
            LifetimeScope.Dispose();

            Assert.False(instance.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_ThrowsOnAttemptToOwn_Singleton()
        {
            Assert.Throws<OwnedScopedComponentException>(() =>
            {
                LifetimeScope.ResolveRequiredService<IOwned<DisposableSingleton>>();
            });
        }

        [Fact]
        public void LifetimeScope_ThrowsOnAttemptToOwn_LifetimeScopedComponent()
        {
            Assert.Throws<OwnedScopedComponentException>(() =>
            {
                LifetimeScope.ResolveRequiredService<IOwned<DisposableLifetimeScopedResource>>();
            });
        }
    }
}