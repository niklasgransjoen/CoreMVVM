using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CoreMVVM.Tests.IOC.Builder
{
    [TestFixture]
    public class ContainerBuilderTests
    {
        private ContainerBuilder _builder;

        [SetUp]
        public void BeforeEach()
        {
            _builder = new ContainerBuilder(registerDefaults: false);
        }

        [TearDown]
        public void AfterEach()
        {
            _builder = null;
        }

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.Register(type);
            ValidateRegistrationBuilder(regBuilder, type, isSingleton: false);
        }

        [Test]
        public void Builder_Registers_Generics()
        {
            IRegistrationBuilder regBuilder = _builder.Register<IInterface>();
            ValidateRegistrationBuilder(regBuilder, typeof(IInterface), isSingleton: false);
        }

        [TestCaseSource(nameof(GetFactories))]
        public void Builder_Registers_Factories(Func<IContainer, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.Register(factory);
            ValidateRegistrationBuilder(regBuilder, typeof(object), isSingleton: false);
        }

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Singletons(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterSingleton(type);
            ValidateRegistrationBuilder(regBuilder, type, isSingleton: true);
        }

        [Test]
        public void Builder_Registers_GenericSingletons()
        {
            IRegistrationBuilder regBuilder = _builder.RegisterSingleton<Class>();
            ValidateRegistrationBuilder(regBuilder, typeof(Class), isSingleton: true);
        }

        [TestCaseSource(nameof(GetFactories))]
        public void Builder_Registers_Singleton_Factories(Func<IContainer, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterSingleton(factory);
            ValidateRegistrationBuilder(regBuilder, typeof(object), isSingleton: true);
        }

        private void ValidateRegistrationBuilder(IRegistrationBuilder regBuilder, Type expectedType, bool isSingleton)
        {
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedType, regBuilder.Type);
                Assert.AreEqual(isSingleton, regBuilder.IsSingleton);
            });
        }

        private static IEnumerable<Type> GetTypes()
        {
            yield return typeof(IInterface);
            yield return typeof(Class);
            yield return typeof(Struct);
        }

        private static IEnumerable<Func<IContainer, object>> GetFactories()
        {
            yield return c => new Class();
            yield return c => new Struct();
            yield return c => new Implementation();
        }
    }
}