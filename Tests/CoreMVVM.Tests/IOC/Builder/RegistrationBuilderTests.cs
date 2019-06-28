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
            RegistrationBuilder regBuilder = _builder.Register<IInterface>();
            regBuilder.As(type);

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(typeof(IInterface), regBuilder.Registrations[type].Type);
        }

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Types_AsSelf(Type type)
        {
            RegistrationBuilder regBuilder = _builder.Register(type).AsSelf();

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(type, regBuilder.Registrations[type].Type);
        }

        private static IEnumerable<Type> GetTypes()
        {
            yield return typeof(IInterface);
            yield return typeof(Class);
            yield return typeof(Struct);
        }
    }
}