using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveSingletonTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterSingleton<Implementation>().As<IInterface>().AsSelf();
            builder.RegisterSingleton<SimpleSingleton>().AsSelf();
            builder.RegisterSingleton<AttributeSingleton>().As<IAttributeSingleton>();

            builder.RegisterSingleton<MultiInterfaceClass>().As<IInterface1>().As<IInterface2>();
        }

        [Fact]
        public void LifetimeScope_ResolvesSingleton_ToSingleInstance()
        {
            object subject1 = LifetimeScope.ResolveRequiredService<IInterface>();
            object subject2 = LifetimeScope.ResolveRequiredService<IInterface>();

            Assert.Same(subject1, subject2);
        }

        [Fact]
        public void LifetimeScope_Resolves_InterfaceAndImplementation_ToSameInstance()
        {
            IInterface s1 = LifetimeScope.ResolveRequiredService<IInterface>();
            Implementation s2 = LifetimeScope.ResolveRequiredService<Implementation>();

            Assert.Same(s1, s2);
        }

        [Fact]
        public void LifetimeScope_ResolvesSingleton_FromSubScope()
        {
            IInterface instance1 = LifetimeScope.ResolveRequiredService<IInterface>();
            IInterface instance2 = LifetimeScope.BeginLifetimeScope().ResolveRequiredService<IInterface>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void LifetimeScope_Resolves_Singleton_By_Attribute()
        {
            var instance1 = LifetimeScope.ResolveRequiredService<AttributeSingleton>();
            var instance2 = LifetimeScope.ResolveRequiredService<AttributeSingleton>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void LifetimeScope_Resolves_Singleton_By_Attribute_And_Registration()
        {
            var instance1 = LifetimeScope.ResolveRequiredService<AttributeSingleton>();
            var instance2 = LifetimeScope.ResolveRequiredService<IAttributeSingleton>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void LifetimeScope_Resolves_Singleton_By_Registration_And_Attribute()
        {
            var instance2 = LifetimeScope.ResolveRequiredService<IAttributeSingleton>();
            var instance1 = LifetimeScope.ResolveRequiredService<AttributeSingleton>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void LifetimeScope_Handles_Multiple_RegistratedInterfaces_With_Attribute_On_Implementations()
        {
            LifetimeScope.ResolveRequiredService<MultiInterfaceClass>();
        }

        [Fact]
        public async Task LifetimeScope_ResolveSingleton_ThreadSafe()
        {
            List<Task<IInterface>> resolvingTasks = new List<Task<IInterface>>
            {
                Task.Run(() => LifetimeScope.ResolveRequiredService<IInterface>()),
                Task.Run(() => LifetimeScope.ResolveRequiredService<IInterface>()),
                Task.Run(() => LifetimeScope.ResolveRequiredService<IInterface>()),
                Task.Run(() => LifetimeScope.ResolveRequiredService<IInterface>()),
                Task.Run(() => LifetimeScope.ResolveRequiredService<IInterface>()),
                Task.Run(() => LifetimeScope.ResolveRequiredService<IInterface>()),
                Task.Run(() => LifetimeScope.ResolveRequiredService<IInterface>()),
            };

            List<IInterface> interfaces = new List<IInterface>();
            foreach (var task in resolvingTasks)
                interfaces.Add(await task);

            while (interfaces.Count > 1)
            {
                for (int i = 1; i < interfaces.Count; i++)
                    Assert.Same(interfaces[0], interfaces[i]);

                interfaces.RemoveAt(0);
            }
        }

        [Fact]
        public void LifetimeScope_SubScopes_Provides_Root_In_Constructor()
        {
            using var subscope = LifetimeScope.BeginLifetimeScope();

            SimpleSingleton disposable = subscope.ResolveRequiredService<SimpleSingleton>();
            Assert.Same(LifetimeScope, disposable.LifetimeScope);
        }

        /**
         * See issue #30.
         */

        [Fact]
        public void LifetimeScope_Handles_Recursive_Scoped_Service_Constructor_Pattern()
        {
            Assert.Throws<ResolveException>(() => LifetimeScope.ResolveRequiredService<SingletonService2>());
        }

        #region Resources

        private interface IAttributeSingleton
        {
        }

        [Scope(ComponentScope.Singleton)]
        private sealed class AttributeSingleton : IAttributeSingleton
        {
        }

        private sealed class SimpleSingleton
        {
            public SimpleSingleton(ILifetimeScope lifetimeScope)
            {
                LifetimeScope = lifetimeScope;
            }

            public ILifetimeScope LifetimeScope { get; }
        }

        private interface IInterface1 { }

        private interface IInterface2 { }

        [Scope(ComponentScope.Singleton)]
        private sealed class MultiInterfaceClass : IInterface1, IInterface2 { }



        [Scope(ComponentScope.Singleton)]
        private sealed class SingletonService2
        {
            public SingletonService2(SingletonService3 singletonService3)
            {
            }
        }

        [Scope(ComponentScope.Singleton)]
        private sealed class SingletonService3
        {
            public SingletonService3(SingletonService2 singletonService2)
            {
            }
        }



        #endregion Resources
    }
}