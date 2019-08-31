using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using CoreMVVM.IOC.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreMVVM.Tests.IOC.Core
{
    public abstract class ContainerTestBase
    {
        protected IContainer Container { get; set; }

        protected virtual void RegisterComponents(ContainerBuilder builder)
        {
        }

        [SetUp]
        public void BeforeEach()
        {
            ContainerBuilder builder = new ContainerBuilder();
            RegisterComponents(builder);

            Container = builder.Build();
        }

        [TearDown]
        public void AfterEach()
        {
            Container = null;
        }
    }

    [TestFixture]
    public class Container_Resolve_Unregistered : ContainerTestBase
    {
        [Test]
        public void Container_CreatesInstance_NoParams()
        {
            Class subject = Container.Resolve<Class>();
            Assert.NotNull(subject);
        }

        [Test]
        public void Container_CreatesInstance_WithParams()
        {
            ClassWithConstructor subject = Container.Resolve<ClassWithConstructor>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.a);
        }

        [Test]
        public void Container_CreatesInstance_ParameterlessConstructor()
        {
            ClassWithEmptyConstructor subject = Container.Resolve<ClassWithEmptyConstructor>();

            Assert.NotNull(subject);
            Assert.IsTrue(subject.constructorWasInvoked);
        }

        [Test]
        public void Container_Calls_ConstructorWithTheMostParams()
        {
            ClassWithManyConstructors subject = Container.Resolve<ClassWithManyConstructors>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Ec1);
            Assert.NotNull(subject.Ec2);
        }

        [Test]
        public void Container_ResolvesContainer_ToSameInstance()
        {
            IContainer c1 = Container.Resolve<IContainer>();
            IContainer c2 = Container.Resolve<IContainer>();

            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void Container_Throws_ResolveUnregisteredInterface()
        {
            try
            {
                Container.Resolve<IInterface>();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ResolveUnregisteredInterfaceException), e.GetType());
            }
        }

        [Test]
        public void Container_Throws_ResolveIllegalTypes()
        {
            try
            {
                Container.Resolve<string>();
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ResolveConstructionException), e.GetType());
            }
        }

        public class ClassWithConstructor
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

        public class ClassWithEmptyConstructor
        {
            public bool constructorWasInvoked;

            public ClassWithEmptyConstructor()
            {
                constructorWasInvoked = true;
            }
        }

        public class ClassWithManyConstructors
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
    public class Container_Resolve : ContainerTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Implementation>().As<IInterface>();
        }

        [Test]
        public void Container_ResolvesInterface_ToRegistration()
        {
            IInterface subject = Container.Resolve<IInterface>();

            Assert.AreEqual(typeof(Implementation), subject.GetType());
        }

        [Test]
        public void Container_ResolvesRegistrations_ToUniqueInstances()
        {
            object subject1 = Container.Resolve<IInterface>();
            object subject2 = Container.Resolve<IInterface>();

            Assert.AreNotEqual(subject1, subject2);
        }
    }

    [TestFixture]
    public class Container_Resolve_Singleton : ContainerTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterSingleton<Implementation>().As<IInterface>().AsSelf();
        }

        [Test]
        public void Container_ResolvesSingleton_ToSingleInstance()
        {
            object subject1 = Container.Resolve<IInterface>();
            object subject2 = Container.Resolve<IInterface>();

            Assert.AreEqual(subject1, subject2);
        }

        [Test]
        public void Container_Resolves_InterfaceAndImplementation_ToSameInstance()
        {
            IInterface s1 = Container.Resolve<IInterface>();
            Implementation s2 = Container.Resolve<Implementation>();

            Assert.AreEqual(s1, s2);
        }

        [Test]
        public async Task Container_ResolveSingleton_ThreadSafe()
        {
            List<Task<IInterface>> resolvingTasks = new List<Task<IInterface>>
            {
                Task.Run(() => Container.Resolve<IInterface>()),
                Task.Run(() => Container.Resolve<IInterface>()),
                Task.Run(() => Container.Resolve<IInterface>()),
                Task.Run(() => Container.Resolve<IInterface>()),
                Task.Run(() => Container.Resolve<IInterface>()),
                Task.Run(() => Container.Resolve<IInterface>()),
                Task.Run(() => Container.Resolve<IInterface>()),
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
    public class Container_Resolve_Factory : ContainerTestBase
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
        public void Container_Resolves_Factory()
        {
            IInterface subject = Container.Resolve<IInterface>();

            Assert.AreEqual(typeof(ClassWithProperties), subject.GetType());
            Assert.AreEqual(4, ((ClassWithProperties)subject).MyVal);
        }

        [Test]
        public void Container_Resolves_Singleton_FromFactory()
        {
            SingletonWithProperties subject1 = Container.Resolve<SingletonWithProperties>();
            SingletonWithProperties subject2 = Container.Resolve<SingletonWithProperties>();

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
}