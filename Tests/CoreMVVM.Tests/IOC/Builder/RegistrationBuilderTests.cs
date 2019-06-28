using CoreMVVM.IOC.Builder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreMVVM.Tests.IOC.Builder
{
    [TestFixture]
    public class RegistrationBuilderTests
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
        public void Builder_Registers_Types(Type type)
        {
            RegistrationBuilder regBuilder = _builder.Register<Class>();
            regBuilder.As(type);

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(typeof(Class), regBuilder.Registrations[type].Type);
        }

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Types_AsSelf(Type type)
        {
            RegistrationBuilder regBuilder = _builder.Register(type).AsSelf();

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(type, regBuilder.Registrations[type].Type);
        }

        [TestCaseSource(nameof(GetFactoriesData))]
        public void Builder_Registers_Types_WithFactory(Type type, Func<object> factory)
        {
            RegistrationBuilder regBuilder = _builder.Register<Class>();
            regBuilder.As(type, factory);

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(factory, regBuilder.Registrations[type].Factory);
        }

        [TestCaseSource(nameof(GetFactoriesData))]
        public void Builder_Registers_Types_AsSelf_WithFactory(Type type, Func<object> factory)
        {
            RegistrationBuilder regBuilder = _builder.Register(type).AsSelf(factory);

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(factory, regBuilder.Registrations[type].Factory);
        }

        private static IEnumerable<Type> GetTypes()
        {
            yield return typeof(IInterface);
            yield return typeof(Class);
            yield return typeof(Struct);
        }

        private static IEnumerable<object[]> GetFactoriesData()
        {
            foreach ((Type type, Func<object> factory) in GetFactories())
                yield return new object[] { type, factory };
        }

        private static IEnumerable<(Type type, Func<object> factory)> GetFactories()
        {
            yield return (typeof(IInterface), () => new Implementation());
            yield return (typeof(Class), () => new Class());
            yield return (typeof(Struct), () => new Struct());
        }
    }
}