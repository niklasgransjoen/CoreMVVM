using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using CoreMVVM.IOC.Core;
using NUnit.Framework;
using System;

namespace CoreMVVM.Tests
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
    public class Container_Register : ContainerTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.Register<Implementation>().As<IInterface>();
            builder.RegisterSingleton<Singleton>().As<ISingleton>();
        }

        [Test]
        public void RegisterTypeFromInterface()
        {
            object subject = Container.Resolve<IInterface>();

            Assert.AreEqual(typeof(Implementation), subject.GetType());
        }

        [Test]
        public void RegisterReturnsDifferentInstances()
        {
            object subject1 = Container.Resolve<IInterface>();
            object subject2 = Container.Resolve<IInterface>();

            Assert.AreNotEqual(subject1, subject2);
        }

        [Test]
        public void RegisterSingletonReturnsSameInstance()
        {
            object subject1 = Container.Resolve<ISingleton>();
            object subject2 = Container.Resolve<ISingleton>();

            Assert.AreEqual(subject1, subject2);
        }

        private interface IInterface { }

        private class Implementation : IInterface { }

        private interface ISingleton { }

        private class Singleton : ISingleton { }
    }

    [TestFixture]
    public class Container_Resolve : ContainerTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterSingleton<Singleton>().As<ISingleton>().AsSelf();
        }

        [Test]
        public void CreatesInstanceWithNoParams()
        {
            EmptyClass subject = Container.Resolve<EmptyClass>();
            Assert.NotNull(subject);
        }

        [Test]
        public void CreateInstanceWithParams()
        {
            ClassWithConstructor subject = Container.Resolve<ClassWithConstructor>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.a);
        }

        [Test]
        public void CreateInstanceWithParameterlessConstructor()
        {
            ClassWithEmptyConstructor subject = Container.Resolve<ClassWithEmptyConstructor>();

            Assert.NotNull(subject);
            Assert.IsTrue(subject.constructorWasInvoked);
        }

        [Test]
        public void UseConstructorWithMostParameters()
        {
            ClassWithManyConstructors subject = Container.Resolve<ClassWithManyConstructors>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Ec1);
            Assert.NotNull(subject.Ec2);
        }

        [Test]
        public void AllowsGenericInitialization()
        {
            EmptyClass subject = Container.Resolve<EmptyClass>();
            Assert.NotNull(subject);
        }

        [Test]
        public void ResolveContainer()
        {
            IContainer c1 = Container.Resolve<IContainer>();
            IContainer c2 = Container.Resolve<IContainer>();

            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void ResolveInterfaceAndImplementationToSamyType()
        {
            ISingleton s1 = Container.Resolve<ISingleton>();
            Singleton s2 = Container.Resolve<Singleton>();

            Assert.AreEqual(s1, s2);
        }

        [Test]
        public void FailOnResolveUnregisteredInterface()
        {
            try
            {
                Container.Resolve<IUnregistered>();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ResolveUnregisteredInterfaceException), e.GetType());
            }
        }

        [Test]
        public void ResolveIllegalType()
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

            public ClassWithManyConstructors(EmptyClass ec1)
            {
                Ec1 = ec1;
            }

            public ClassWithManyConstructors(EmptyClass ec1, EmptyClass ec2)
            {
                Ec1 = ec1;
                Ec2 = ec2;
            }

            public EmptyClass Ec1 { get; }
            public EmptyClass Ec2 { get; }
        }

        public interface IUnregistered { }

        public interface ISingleton { }

        public class Singleton : ISingleton { }
    }
}