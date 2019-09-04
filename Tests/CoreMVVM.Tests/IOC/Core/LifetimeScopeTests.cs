using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreMVVM.Tests.IOC.Core
{
    public abstract class LifetimeScopeTestBase
    {
        protected ILifetimeScope LifetimeScope { get; set; }

        protected virtual void RegisterComponents(ContainerBuilder builder)
        {
        }

        [SetUp]
        public void BeforeEach()
        {
            ContainerBuilder builder = new ContainerBuilder();
            RegisterComponents(builder);

            LifetimeScope = builder.Build();
        }

        [TearDown]
        public void AfterEach()
        {
            LifetimeScope.Dispose();
            LifetimeScope = null;
        }
    }

    [TestFixture]
    public class LifetimeScope_Resolve : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Implementation>().As<IInterface>();
        }

        [Test]
        public void LifetimeScope_ResolvesInterface_ToRegistration()
        {
            IInterface subject = LifetimeScope.Resolve<IInterface>();

            Assert.AreEqual(typeof(Implementation), subject.GetType());
        }

        [Test]
        public void LifetimeScope_ResolvesRegistrations_ToUniqueInstances()
        {
            object subject1 = LifetimeScope.Resolve<IInterface>();
            object subject2 = LifetimeScope.Resolve<IInterface>();

            Assert.AreNotEqual(subject1, subject2);
        }
    }

    [TestFixture]
    public class LifetimeScope_Resolve_Singleton : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterSingleton<Implementation>().As<IInterface>().AsSelf();
        }

        [Test]
        public void LifetimeScope_ResolvesSingleton_ToSingleInstance()
        {
            object subject1 = LifetimeScope.Resolve<IInterface>();
            object subject2 = LifetimeScope.Resolve<IInterface>();

            Assert.AreEqual(subject1, subject2);
        }

        [Test]
        public void LifetimeScope_Resolves_InterfaceAndImplementation_ToSameInstance()
        {
            IInterface s1 = LifetimeScope.Resolve<IInterface>();
            Implementation s2 = LifetimeScope.Resolve<Implementation>();

            Assert.AreEqual(s1, s2);
        }

        [Test]
        public void LifetimeScope_ResolvesSingleton_FromLifetimeScope()
        {
            IInterface instance1 = LifetimeScope.Resolve<IInterface>();
            IInterface instance2 = LifetimeScope.BeginLifetimeScope().Resolve<IInterface>();

            Assert.AreEqual(instance1, instance2);
        }

        [Test]
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
                    Assert.AreEqual(interfaces[0], interfaces[i]);

                interfaces.RemoveAt(0);
            }
        }
    }

    [TestFixture]
    public class LifetimeScope_Resolves_LifetimeScope : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterLifetimeScope<Implementation>().As<IInterface>().AsSelf();
        }

        [Test]
        public void LifetimeScope_ResolvesFromRoot_ToSingleInstance()
        {
            object subject1 = LifetimeScope.Resolve<IInterface>();
            object subject2 = LifetimeScope.Resolve<IInterface>();

            Assert.AreEqual(subject1, subject2);
        }

        [Test]
        public void LifetimeScope_ResolvesFromSubscope_ToSingleInstance()
        {
            using (ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope())
            {
                object subject1 = subscope.Resolve<IInterface>();
                object subject2 = subscope.Resolve<IInterface>();

                Assert.AreEqual(subject1, subject2);
            }
        }

        [Test]
        public void LifetimeScope_Resolves_UniqueInstance_FromDifferentScopes()
        {
            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();

            object subject1 = LifetimeScope.Resolve<IInterface>();
            object subject2 = subscope.Resolve<IInterface>();

            Assert.AreNotEqual(subject1, subject2);
        }

        [Test]
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
                    Assert.AreEqual(interfaces[0], interfaces[i]);

                interfaces.RemoveAt(0);
            }
        }
    }

    [TestFixture]
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

        [Test]
        public void LifetimeScope_Resolves_Factory()
        {
            IInterface subject = LifetimeScope.Resolve<IInterface>();

            Assert.AreEqual(typeof(ClassWithProperties), subject.GetType());
            Assert.AreEqual(4, ((ClassWithProperties)subject).MyVal);
        }

        [Test]
        public void LifetimeScope_Resolves_Singleton_FromFactory()
        {
            SingletonWithProperties subject1 = LifetimeScope.Resolve<SingletonWithProperties>();
            SingletonWithProperties subject2 = LifetimeScope.Resolve<SingletonWithProperties>();

            Assert.AreEqual(subject1, subject2);
            Assert.AreEqual("unique-string", subject1.Str);
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

    [TestFixture]
    public class LifetimeScope_Resolve_Func : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Implementation>().As<IInterface>();
            builder.Register(c => (Class)null).AsSelf();
        }

        [Test]
        public void LifetimeScope_Resolves_Func()
        {
            Func<IInterface> factory = LifetimeScope.Resolve<Func<IInterface>>();

            Assert.NotNull(factory);
            Assert.AreEqual(typeof(Func<IInterface>), factory.GetType());

            object instance = factory();
            Assert.AreEqual(typeof(Implementation), instance.GetType());
        }

        [Test]
        public void LifetimeScope_Handles_NullReturningFunc()
        {
            var instance = LifetimeScope.Resolve<Class>();
            Assert.IsNull(instance);
        }
    }

    [TestFixture]
    public class LifetimeScope_Resolve_Owned : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Implementation>().As<IInterface>();

            builder.Register<Disposable>().As<IDisposableInterface>();
            builder.RegisterSingleton<DisposableSingleton>().AsSelf();
            builder.RegisterLifetimeScope<DisposableLifetimeScopedResource>().AsSelf();
        }

        [Test]
        public void LifetimeScope_Resolves_Owned()
        {
            Owned<IInterface> ownedInstance = LifetimeScope.Resolve<Owned<IInterface>>();

            Assert.NotNull(ownedInstance);
            Assert.NotNull(ownedInstance.Value);
        }

        [Test]
        public void LifetimeScope_Resolves_IOwned()
        {
            IOwned<IInterface> ownedInstance = LifetimeScope.Resolve<IOwned<IInterface>>();

            Assert.NotNull(ownedInstance);
            Assert.NotNull(ownedInstance.Value);
        }

        [Test]
        public void LifetimeScope_DoesNotDisposed_OwnedComponent()
        {
            Owned<IDisposableInterface> disposable = LifetimeScope.Resolve<Owned<IDisposableInterface>>();

            Assert.IsFalse(disposable.Value.IsDisposed);

            LifetimeScope.Dispose();
            Assert.IsFalse(disposable.Value.IsDisposed);
        }

        [Test]
        public void LifetimeScope_DoesNotOwn_FuncResult()
        {
            Func<IDisposableInterface> factory = LifetimeScope.Resolve<Func<IDisposableInterface>>();
            IDisposableInterface instance = factory();

            Assert.IsFalse(instance.IsDisposed);
            LifetimeScope.Dispose();

            Assert.IsTrue(instance.IsDisposed);
        }

        [Test]
        public void LifetimeScope_Owns_FuncResult()
        {
            var factory = LifetimeScope.Resolve<IOwned<Func<IDisposableInterface>>>();
            IDisposableInterface instance = factory.Value();

            Assert.IsFalse(instance.IsDisposed);
            LifetimeScope.Dispose();

            Assert.IsFalse(instance.IsDisposed);
        }

        [Test]
        public void LifetimeScope_ThrowsOnAttemptToOwn_Singleton()
        {
            Assert.Throws<OwnedScopedComponentException>(() =>
            {
                LifetimeScope.Resolve<IOwned<DisposableSingleton>>();
            });
        }

        [Test]
        public void LifetimeScope_ThrowsOnAttemptToOwn_LifetimeScopedComponent()
        {
            Assert.Throws<OwnedScopedComponentException>(() =>
            {
                LifetimeScope.Resolve<IOwned<DisposableLifetimeScopedResource>>();
            });
        }
    }

    [TestFixture]
    public class LifetimeScope_Resolve_Unregistered : LifetimeScopeTestBase
    {
        [Test]
        public void LifetimeScope_CreatesInstance_NoParams()
        {
            Class subject = LifetimeScope.Resolve<Class>();
            Assert.NotNull(subject);
        }

        [Test]
        public void LifetimeScope_CreatesInstance_WithParams()
        {
            ClassWithConstructor subject = LifetimeScope.Resolve<ClassWithConstructor>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.a);
        }

        [Test]
        public void LifetimeScope_CreatesInstance_ParameterlessConstructor()
        {
            ClassWithEmptyConstructor subject = LifetimeScope.Resolve<ClassWithEmptyConstructor>();

            Assert.NotNull(subject);
            Assert.IsTrue(subject.constructorWasInvoked);
        }

        [Test]
        public void LifetimeScope_Calls_ConstructorWithTheMostParams()
        {
            ClassWithManyConstructors subject = LifetimeScope.Resolve<ClassWithManyConstructors>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Ec1);
            Assert.NotNull(subject.Ec2);
        }

        [Test]
        public void LifetimeScope_ResolvesContainer_ToSameInstance()
        {
            IContainer c1 = LifetimeScope.Resolve<IContainer>();
            IContainer c2 = LifetimeScope.Resolve<IContainer>();

            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void LifetimeScope_Throws_ResolveUnregisteredInterface()
        {
            try
            {
                LifetimeScope.Resolve<IInterface>();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ResolveUnregisteredInterfaceException), e.GetType());
            }
        }

        [Test]
        public void LifetimeScope_Throws_ResolveIllegalTypes()
        {
            Assert.Throws(typeof(ResolveConstructionException), () =>
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

    [TestFixture]
    public class LifetimeScope_Dispose_Component : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Disposable>().As<IDisposableInterface>();
            builder.RegisterSingleton<DisposableSingleton>().AsSelf();
            builder.RegisterLifetimeScope<DisposableLifetimeScopedResource>().AsSelf();
        }

        [Test]
        public void LifetimeScope_Disposes_IDisposables()
        {
            var instance = LifetimeScope.Resolve<IDisposableInterface>();

            Assert.IsFalse(instance.IsDisposed);
            LifetimeScope.Dispose();

            Assert.IsTrue(instance.IsDisposed);
        }

        [Test]
        public void LifetimeScope_Only_Disposes_Children()
        {
            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();

            var instance = LifetimeScope.Resolve<Disposable>();
            var subInstance = subscope.Resolve<Disposable>();

            Assert.IsFalse(subInstance.IsDisposed);
            subscope.Dispose();

            Assert.IsTrue(subInstance.IsDisposed);
            Assert.IsFalse(instance.IsDisposed);
        }

        [Test]
        public void LifetimeScope_Disposes_SubScopes()
        {
            ILifetimeScope subscope = LifetimeScope.BeginLifetimeScope();

            Assert.IsFalse(LifetimeScope.IsDisposed);
            Assert.IsFalse(subscope.IsDisposed);

            LifetimeScope.Dispose();

            Assert.IsTrue(LifetimeScope.IsDisposed);
            Assert.IsTrue(subscope.IsDisposed);
        }

        [Test]
        public void LifetimeScope_Disposes_Singleton()
        {
            var disposable = LifetimeScope.Resolve<DisposableSingleton>();

            Assert.IsFalse(disposable.IsDisposed);
            LifetimeScope.Dispose();

            Assert.IsTrue(disposable.IsDisposed);
        }

        [Test]
        public void LifetimeScope_Disposes_LifetimeScope()
        {
            var disposable = LifetimeScope.Resolve<DisposableLifetimeScopedResource>();

            Assert.IsFalse(disposable.IsDisposed);
            LifetimeScope.Dispose();

            Assert.IsTrue(disposable.IsDisposed);
        }
    }

    [TestFixture]
    public class LifetimeScope_Initialization : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<InitClass>().AsSelf();
            builder.Register(c => new InitClass()).As<IInterface>();

            builder.RegisterSingleton<InitClass2>().As<IInit>();
        }

        [Test]
        public void LifetimeScope_Initializes_Resolved_Component()
        {
            var instance = LifetimeScope.Resolve<InitClass>();

            Assert.IsTrue(instance.IsInitialized);
        }

        [Test]
        public void LifetimeScope_Initilizes_FuncResult()
        {
            var factory = LifetimeScope.Resolve<Func<InitClass>>();
            var instance = factory();

            Assert.IsTrue(instance.IsInitialized);
        }

        [Test]
        public void LifetimeScope_Initializes_FactoryResult()
        {
            var instance = (InitClass)LifetimeScope.Resolve<IInterface>();

            Assert.IsTrue(instance.IsInitialized);
        }

        [Test]
        public void LifetimeScope_RegistersSingleton_Before_Initializing()
        {
            LifetimeScope.Resolve<IInit>();
        }

        [Test]
        public void LifetimeScope_Initializes_NestedTypes_Once()
        {
            InitClass2 instance = (InitClass2)LifetimeScope.Resolve<IInit>();

            Assert.AreEqual(1, instance.InitClass.InitializationCount);
        }

        public interface IInit : IComponent
        {
            bool IsInitialized { get; }
        }

        public class InitClass : IInit, IInterface
        {
            public void InitializeComponent()
            {
                IsInitialized = true;
                InitializationCount++;
            }

            public bool IsInitialized { get; set; }
            public int InitializationCount { get; private set; }
        }

        public class InitClass2 : IInit
        {
            private readonly IContainer _container;

            public InitClass2(IContainer container, InitClass initClass)
            {
                _container = container;
                InitClass = initClass;
            }

            public void InitializeComponent()
            {
                _container.Resolve<IInit>();
                IsInitialized = true;
            }

            public bool IsInitialized { get; set; }
            public InitClass InitClass { get; }
        }
    }
}