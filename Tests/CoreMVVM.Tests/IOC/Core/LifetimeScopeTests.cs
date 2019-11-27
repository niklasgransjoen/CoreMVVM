using CoreMVVM.Implementations;
using CoreMVVM.IOC.Builder;
using CoreMVVM.Services;
using CoreMVVM.Tests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public abstract class LifetimeScopeTestBase : IDisposable
    {
        protected LifetimeScopeTestBase()
        {
            ContainerBuilder builder = new ContainerBuilder();
            RegisterComponents(builder);

            LifetimeScope = builder.Build();
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

    #region Scope

    public class LifetimeScope_Resolve : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Implementation>().As<IInterface>();
        }

        [Fact]
        public void LifetimeScope_ResolvesInterface_ToRegistration()
        {
            IInterface subject = LifetimeScope.Resolve<IInterface>();

            Assert.IsType<Implementation>(subject);
        }

        [Fact]
        public void LifetimeScope_ResolvesRegistrations_ToUniqueInstances()
        {
            object subject1 = LifetimeScope.Resolve<IInterface>();
            object subject2 = LifetimeScope.Resolve<IInterface>();

            Assert.NotEqual(subject1, subject2);
        }
    }

    public class LifetimeScope_Resolve_Singleton : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterSingleton<Implementation>().As<IInterface>().AsSelf();
            builder.RegisterSingleton<SimpleSingleton>().AsSelf();
        }

        [Fact]
        public void LifetimeScope_ResolvesSingleton_ToSingleInstance()
        {
            object subject1 = LifetimeScope.Resolve<IInterface>();
            object subject2 = LifetimeScope.Resolve<IInterface>();

            Assert.Equal(subject1, subject2);
        }

        [Fact]
        public void LifetimeScope_Resolves_InterfaceAndImplementation_ToSameInstance()
        {
            IInterface s1 = LifetimeScope.Resolve<IInterface>();
            Implementation s2 = LifetimeScope.Resolve<Implementation>();

            Assert.Equal(s1, s2);
        }

        [Fact]
        public void LifetimeScope_ResolvesSingleton_FromLifetimeScope()
        {
            IInterface instance1 = LifetimeScope.Resolve<IInterface>();
            IInterface instance2 = LifetimeScope.BeginLifetimeScope().Resolve<IInterface>();

            Assert.Equal(instance1, instance2);
        }

        [Fact]
        public async Task LifetimeScope_ResolveSingleton_ThreadSafe()
        {
            List<Task<IInterface>> resolvingTasks = new List<Task<IInterface>>
            {
                Task.Run(() => LifetimeScope.Resolve<IInterface>()),
                Task.Run(() => LifetimeScope.Resolve<IInterface>()),
                Task.Run(() => LifetimeScope.Resolve<IInterface>()),
                Task.Run(() => LifetimeScope.Resolve<IInterface>()),
                Task.Run(() => LifetimeScope.Resolve<IInterface>()),
                Task.Run(() => LifetimeScope.Resolve<IInterface>()),
                Task.Run(() => LifetimeScope.Resolve<IInterface>()),
            };

            List<IInterface> interfaces = new List<IInterface>();
            foreach (var task in resolvingTasks)
                interfaces.Add(await task);

            while (interfaces.Count > 1)
            {
                for (int i = 1; i < interfaces.Count; i++)
                    Assert.Equal(interfaces[0], interfaces[i]);

                interfaces.RemoveAt(0);
            }
        }

        [Fact]
        public void LifetimeScope_SubScopes_Provides_Root_In_Constructor()
        {
            using var subscope = LifetimeScope.BeginLifetimeScope();

            SimpleSingleton disposable = subscope.Resolve<SimpleSingleton>();
            Assert.Same(LifetimeScope, disposable.LifetimeScope);
        }

        private sealed class SimpleSingleton
        {
            public SimpleSingleton(ILifetimeScope lifetimeScope)
            {
                LifetimeScope = lifetimeScope;
            }

            public ILifetimeScope LifetimeScope { get; }
        }
    }

    public class LifetimeScope_Resolve_LifetimeScope : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterLifetimeScope<Implementation>().As<IInterface>().AsSelf();
        }

        [Fact]
        public void LifetimeScope_ResolvesFromRoot_ToSingleInstance()
        {
            object subject1 = LifetimeScope.Resolve<IInterface>();
            object subject2 = LifetimeScope.Resolve<IInterface>();

            Assert.Equal(subject1, subject2);
        }

        [Fact]
        public void LifetimeScope_ResolvesFromSubscope_ToSingleInstance()
        {
            using ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();

            object subject1 = subscope.Resolve<IInterface>();
            object subject2 = subscope.Resolve<IInterface>();

            Assert.Equal(subject1, subject2);
        }

        [Fact]
        public void LifetimeScope_Resolves_UniqueInstance_FromDifferentScopes()
        {
            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();

            object subject1 = LifetimeScope.Resolve<IInterface>();
            object subject2 = subscope.Resolve<IInterface>();

            Assert.NotEqual(subject1, subject2);
        }

        [Fact]
        public async Task LifetimeScope_ResolveSingleton_ThreadSafe()
        {
            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();
            List<Task<IInterface>> resolvingTasks = new List<Task<IInterface>>
            {
                Task.Run(() => subscope.Resolve<IInterface>()),
                Task.Run(() => subscope.Resolve<IInterface>()),
                Task.Run(() => subscope.Resolve<IInterface>()),
                Task.Run(() => subscope.Resolve<IInterface>()),
                Task.Run(() => subscope.Resolve<IInterface>()),
                Task.Run(() => subscope.Resolve<IInterface>()),
                Task.Run(() => subscope.Resolve<IInterface>()),
            };

            List<IInterface> interfaces = new List<IInterface>();
            foreach (var task in resolvingTasks)
                interfaces.Add(await task);

            while (interfaces.Count > 1)
            {
                for (int i = 1; i < interfaces.Count; i++)
                    Assert.Equal(interfaces[0], interfaces[i]);

                interfaces.RemoveAt(0);
            }
        }
    }

    #endregion Scope

    #region Resolve Factory

    public class LifetimeScope_Resolve_Factory : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register(c => new ClassWithProperties
            {
                MyVal = 4,
            }).As<IInterface>();

            builder.RegisterSingleton(c => new SingletonWithProperties()
            {
                Str = "unique-string",
            }).AsSelf();
        }

        [Fact]
        public void LifetimeScope_Resolves_Factory()
        {
            IInterface subject = LifetimeScope.Resolve<IInterface>();

            Assert.IsType<ClassWithProperties>(subject);
            Assert.Equal(4, ((ClassWithProperties)subject).MyVal);
        }

        [Fact]
        public void LifetimeScope_Resolves_Singleton_FromFactory()
        {
            SingletonWithProperties subject1 = LifetimeScope.Resolve<SingletonWithProperties>();
            SingletonWithProperties subject2 = LifetimeScope.Resolve<SingletonWithProperties>();

            Assert.Equal(subject1, subject2);
            Assert.Equal("unique-string", subject1.Str);
        }

        private class ClassWithProperties : IInterface
        {
            public int MyVal { get; set; }
        }

        private class SingletonWithProperties
        {
            public string Str { get; set; }
        }
    }

    #endregion Resolve Factory

    #region Resolve self

    public class LifetimeScope_Resolve_Self : LifetimeScopeTestBase
    {
        [Fact]
        public void LifetimeScope_Resolves_Self()
        {
            ILifetimeScope res1 = LifetimeScope.Resolve<ILifetimeScope>();
            Assert.Equal(LifetimeScope, res1);

            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();
            ILifetimeScope res2 = subscope.Resolve<ILifetimeScope>();
            Assert.Equal(subscope, res2);
        }
    }

    #endregion Resolve self

    #region Generic result

    public class LifetimeScope_Resolve_Func : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Implementation>().As<IInterface>();
            builder.Register(c => (Class)null).AsSelf();
        }

        [Fact]
        public void LifetimeScope_Resolves_Func()
        {
            Func<IInterface> factory = LifetimeScope.Resolve<Func<IInterface>>();

            Assert.NotNull(factory);
            Assert.IsType<Func<IInterface>>(factory);

            object instance = factory();
            Assert.IsType<Implementation>(instance);
        }

        [Fact]
        public void LifetimeScope_Handles_NullReturningFunc()
        {
            var instance = LifetimeScope.Resolve<Class>();
            Assert.Null(instance);
        }
    }

    public class LifetimeScope_Resolve_Lazy : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Implementation>().As<IInterface>();
        }

        [Fact]
        public void LifetimeScope_Resolves_Lazy()
        {
            Lazy<IInterface> lazyInstance = LifetimeScope.Resolve<Lazy<IInterface>>();

            Assert.False(lazyInstance.IsValueCreated);

            IInterface instance = lazyInstance.Value;

            Assert.IsType<Implementation>(instance);
            Assert.True(lazyInstance.IsValueCreated);
        }

        [Fact]
        public void LifetimeScope_Resolves_UnregistratedLazy()
        {
            Lazy<Class> lazyInstance = LifetimeScope.Resolve<Lazy<Class>>();

            Assert.False(lazyInstance.IsValueCreated);

            Class instance = lazyInstance.Value;

            Assert.IsType<Class>(instance);
            Assert.True(lazyInstance.IsValueCreated);
        }

        [Fact]
        public void LifetimeScope_Resolves_UnregistratedLazyWithParameters()
        {
            Lazy<MyClass> lazyInstance = LifetimeScope.Resolve<Lazy<MyClass>>();

            Assert.False(lazyInstance.IsValueCreated);

            MyClass instance = lazyInstance.Value;

            Assert.IsType<MyClass>(instance);
            Assert.NotNull(instance.C);
            Assert.True(lazyInstance.IsValueCreated);
        }

        [Fact]
        public void LifetimeScope_Resolves_LazyFunc()
        {
            Lazy<Func<IInterface>> lazyFactory = LifetimeScope.Resolve<Lazy<Func<IInterface>>>();

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

    public class LifetimeScope_Resolve_Owned : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Implementation>().As<IInterface>();

            builder.Register<Disposable>().As<IDisposableInterface>();
            builder.RegisterSingleton<DisposableSingleton>().AsSelf();
            builder.RegisterLifetimeScope<DisposableLifetimeScopedResource>().AsSelf();
        }

        [Fact]
        public void LifetimeScope_Resolves_Owned()
        {
            Owned<IInterface> ownedInstance = LifetimeScope.Resolve<Owned<IInterface>>();

            Assert.NotNull(ownedInstance);
            Assert.NotNull(ownedInstance.Value);
        }

        [Fact]
        public void LifetimeScope_Resolves_IOwned()
        {
            IOwned<IInterface> ownedInstance = LifetimeScope.Resolve<IOwned<IInterface>>();

            Assert.NotNull(ownedInstance);
            Assert.NotNull(ownedInstance.Value);
        }

        [Fact]
        public void LifetimeScope_DoesNotDisposed_OwnedComponent()
        {
            Owned<IDisposableInterface> disposable = LifetimeScope.Resolve<Owned<IDisposableInterface>>();

            Assert.False(disposable.Value.IsDisposed);

            LifetimeScope.Dispose();
            Assert.False(disposable.Value.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_DoesNotOwn_FuncResult()
        {
            Func<IDisposableInterface> factory = LifetimeScope.Resolve<Func<IDisposableInterface>>();
            IDisposableInterface instance = factory();

            Assert.False(instance.IsDisposed);
            LifetimeScope.Dispose();

            Assert.True(instance.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_Owns_FuncResult()
        {
            var factory = LifetimeScope.Resolve<IOwned<Func<IDisposableInterface>>>();
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
                LifetimeScope.Resolve<IOwned<DisposableSingleton>>();
            });
        }

        [Fact]
        public void LifetimeScope_ThrowsOnAttemptToOwn_LifetimeScopedComponent()
        {
            Assert.Throws<OwnedScopedComponentException>(() =>
            {
                LifetimeScope.Resolve<IOwned<DisposableLifetimeScopedResource>>();
            });
        }
    }

    #endregion Generic result

    #region Resolve Unregistered

    public class LifetimeScope_Resolve_Unregistered : LifetimeScopeTestBase
    {
        [Fact]
        public void LifetimeScope_CreatesInstance_NoParams()
        {
            Class subject = LifetimeScope.Resolve<Class>();
            Assert.NotNull(subject);
        }

        [Fact]
        public void LifetimeScope_CreatesInstance_WithParams()
        {
            ClassWithConstructor subject = LifetimeScope.Resolve<ClassWithConstructor>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.a);
        }

        [Fact]
        public void LifetimeScope_CreatesInstance_ParameterlessConstructor()
        {
            ClassWithEmptyConstructor subject = LifetimeScope.Resolve<ClassWithEmptyConstructor>();

            Assert.NotNull(subject);
            Assert.True(subject.constructorWasInvoked);
        }

        [Fact]
        public void LifetimeScope_Calls_ConstructorWithTheMostParams()
        {
            ClassWithManyConstructors subject = LifetimeScope.Resolve<ClassWithManyConstructors>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Ec1);
            Assert.NotNull(subject.Ec2);
        }

        [Fact]
        public void LifetimeScope_ResolvesContainer_ToSameInstance()
        {
            IContainer c1 = LifetimeScope.Resolve<IContainer>();
            IContainer c2 = LifetimeScope.Resolve<IContainer>();

            Assert.Equal(c1, c2);
        }

        [Fact]
        public void LifetimeScope_Throws_ResolveUnregisteredInterface()
        {
            Assert.Throws<ResolveUnregisteredInterfaceException>(() => LifetimeScope.Resolve<IInterface>());
        }

        [Fact(Skip = "This doesn't throw, but maybe it should")]
        public void LifetimeScope_Throws_ResolveIllegalTypes()
        {
            Assert.Throws<ResolveConstructionException>(() =>
            {
                LifetimeScope.Resolve<string>();
            });
        }

        internal class ClassWithConstructor
        {
            public Class a;

            public ClassWithConstructor()
            {
            }

            public ClassWithConstructor(Class a)
            {
                this.a = a;
            }
        }

        internal class ClassWithEmptyConstructor
        {
            public bool constructorWasInvoked;

            public ClassWithEmptyConstructor()
            {
                constructorWasInvoked = true;
            }
        }

        internal class ClassWithManyConstructors
        {
            public ClassWithManyConstructors()
            {
            }

            public ClassWithManyConstructors(Class ec1)
            {
                Ec1 = ec1;
            }

            public ClassWithManyConstructors(Class ec1, Class ec2)
            {
                Ec1 = ec1;
                Ec2 = ec2;
            }

            public Class Ec1 { get; }
            public Class Ec2 { get; }
        }
    }

    #endregion Resolve Unregistered

    #region Dispose

    public class LifetimeScope_Dispose_Component : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Disposable>().As<IDisposableInterface>();
            builder.RegisterSingleton<DisposableSingleton>().AsSelf();
            builder.RegisterLifetimeScope<DisposableLifetimeScopedResource>().AsSelf();
        }

        [Fact]
        public void LifetimeScope_Disposes_IDisposables()
        {
            var instance = LifetimeScope.Resolve<IDisposableInterface>();

            Assert.False(instance.IsDisposed);
            LifetimeScope.Dispose();

            Assert.True(instance.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_Only_Disposes_Children()
        {
            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();

            var instance = LifetimeScope.Resolve<Disposable>();
            var subInstance = subscope.Resolve<Disposable>();

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
            var disposable = LifetimeScope.Resolve<DisposableSingleton>();

            Assert.False(disposable.IsDisposed);
            LifetimeScope.Dispose();

            Assert.True(disposable.IsDisposed);
        }

        [Fact]
        public void LifetimeScope_Disposes_LifetimeScope()
        {
            var disposable = LifetimeScope.Resolve<DisposableLifetimeScopedResource>();

            Assert.False(disposable.IsDisposed);
            LifetimeScope.Dispose();

            Assert.True(disposable.IsDisposed);
        }
    }

    #endregion Dispose

    #region Initializes Component

    public class LifetimeScope_Initialization : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<InitClass>().AsSelf();
            builder.Register(c => new InitClass()).As<IInterface>();

            builder.RegisterSingleton<InitClass2>().As<IInit>();
        }

        [Fact]
        public void LifetimeScope_Initializes_Resolved_Component()
        {
            var instance = LifetimeScope.Resolve<InitClass>();

            Assert.True(instance.IsInitialized);
        }

        [Fact]
        public void LifetimeScope_Initilizes_FuncResult()
        {
            var factory = LifetimeScope.Resolve<Func<InitClass>>();
            var instance = factory();

            Assert.True(instance.IsInitialized);
        }

        [Fact]
        public void LifetimeScope_Initializes_FactoryResult()
        {
            var instance = (InitClass)LifetimeScope.Resolve<IInterface>();

            Assert.True(instance.IsInitialized);
        }

        [Fact]
        public void LifetimeScope_RegistersSingleton_Before_Initializing()
        {
            LifetimeScope.Resolve<IInit>();
        }

        [Fact]
        public void LifetimeScope_Initializes_NestedTypes_Once()
        {
            InitClass2 instance = (InitClass2)LifetimeScope.Resolve<IInit>();

            Assert.Equal(1, instance.InitClass.InitializationCount);
        }

        private interface IInit : IComponent
        {
            bool IsInitialized { get; }
        }

        private class InitClass : IInit, IInterface
        {
            public void InitializeComponent(ILifetimeScope lifetimeScope)
            {
                IsInitialized = true;
                InitializationCount++;
            }

            public bool IsInitialized { get; set; }
            public int InitializationCount { get; private set; }
        }

        private class InitClass2 : IInit
        {
            public InitClass2(InitClass initClass)
            {
                InitClass = initClass;
            }

            public void InitializeComponent(ILifetimeScope lifetimeScope)
            {
                lifetimeScope.Resolve<IInit>();
                IsInitialized = true;
            }

            public bool IsInitialized { get; set; }
            public InitClass InitClass { get; }
        }
    }

    #endregion Initializes Component

    #region Fallback Implementation

    public sealed class LifetimeScope_Handles_Fallback : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
        }

        [Fact]
        public void LifetimeScope_Resolves_Unregistered_Logger()
        {
            var logger = LifetimeScope.Resolve<ILogger>();
            Assert.IsType<ConsoleLogger>(logger);
        }

        #region Cache

        [Fact]
        public void LifetimeScope_Fallback_Caches()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterSingleton<ResolveLoggerService>().As<IResolveUnregisteredInterfaceService>();
            IContainer container = builder.Build();

            var service = (ResolveLoggerService)container.Resolve<IResolveUnregisteredInterfaceService>();
            service.Cache = true;

            container.Resolve<ILogger>();
            container.Resolve<ILogger>();

            // Assert that resolve service was only invoked once.
            Assert.Equal(1, service.CallCount);
        }

        #endregion Cache

        #region Don't cache

        [Fact]
        public void LifetimeScope_Fallback_DoesNot_Cache()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterSingleton<ResolveLoggerService>().As<IResolveUnregisteredInterfaceService>();
            IContainer container = builder.Build();

            var service = (ResolveLoggerService)container.Resolve<IResolveUnregisteredInterfaceService>();
            service.Cache = false;

            container.Resolve<ILogger>();
            container.Resolve<ILogger>();

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
                context.SetInterfaceImplementationType(typeof(ConsoleLogger));
                context.CacheImplementation = Cache;
            }
        }
    }

    #endregion Fallback Implementation
}