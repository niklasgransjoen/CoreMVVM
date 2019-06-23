using NUnit.Framework;

namespace CoreMVVM.Tests
{
    public abstract class ContainerTestBase
    {
        protected IContainer container;

        [SetUp]
        public void BeforeEach()
        {
            ContainerBuilder builder = new ContainerBuilder();
            RegisterComponents(builder);

            container = builder.Build();
        }

        protected virtual void RegisterComponents(ContainerBuilder builder)
        {
        }

        [TearDown]
        public void AfterEach()
        {
            container = null;
        }
    }

    [TestFixture]
    public class Container_Register : ContainerTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<IInterface, Implementation>();
            builder.RegisterSingleton<ISingleton, Singleton>();
        }

        [Test]
        public void RegisterTypeFromInterface()
        {
            object subject = container.Resolve<IInterface>();

            Assert.AreEqual(typeof(Implementation), subject.GetType());
        }

        [Test]
        public void RegisterDoesNotReturnSingleton()
        {
            object subject1 = container.Resolve<IInterface>();
            object subject2 = container.Resolve<IInterface>();

            Assert.AreNotEqual(subject1, subject2);
        }

        [Test]
        public void RegisterSingleton()
        {
            object subject1 = container.Resolve<ISingleton>();
            object subject2 = container.Resolve<ISingleton>();

            Assert.AreEqual(subject1, subject2);
        }

        private interface IInterface
        {
            int Property { get; }
        }

        private class Implementation : IInterface
        {
            public int Property { get; }
        }

        private interface ISingleton
        {
        }

        private class Singleton : ISingleton
        {
        }
    }

    [TestFixture]
    public class Container_Resolve : ContainerTestBase
    {
        [Test]
        public void CreatesInstanceWithNoParams()
        {
            object subject = container.Resolve(typeof(EmptyClass));
            Assert.AreEqual(typeof(EmptyClass), subject.GetType());
        }

        [Test]
        public void CreateInstanceWithParams()
        {
            object subject = container.Resolve(typeof(ClassWithConstructor));

            Assert.IsTrue(subject.GetType() == typeof(ClassWithConstructor));
            Assert.NotNull(((ClassWithConstructor)subject).a);
        }

        [Test]
        public void CreateInstanceWithParameterlessConstructor()
        {
            object subject = container.Resolve(typeof(ClassWithEmptyConstructor));

            Assert.IsTrue(subject.GetType() == typeof(ClassWithEmptyConstructor));
            Assert.IsTrue(((ClassWithEmptyConstructor)subject).invoked);
        }

        [Test]
        public void AllowsGenericInitialization()
        {
            EmptyClass subject = container.Resolve<EmptyClass>();
            Assert.NotNull(subject);
        }

        [Test]
        public void ResolveContainer()
        {
            IContainer c1 = container.Resolve<IContainer>();
            IContainer c2 = container.Resolve<IContainer>();

            Assert.AreEqual(c1, c2);
        }

        public class EmptyClass { }

        public class ClassWithConstructor
        {
            public EmptyClass a;

            public ClassWithConstructor()
            {
            }

            public ClassWithConstructor(EmptyClass a)
            {
                this.a = a;
            }
        }

        public class ClassWithEmptyConstructor
        {
            public bool invoked;

            public ClassWithEmptyConstructor()
            {
                invoked = true;
            }
        }
    }
}