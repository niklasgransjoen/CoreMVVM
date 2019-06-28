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

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Types(Type type)
        {
            RegistrationBuilder regBuilder = _builder.Register(type);
            ValidateRegistrationBuilder(regBuilder, type, isSingleton: false);
        }

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Types_AsSingletons(Type type)
        {
            RegistrationBuilder regBuilder = _builder.RegisterSingleton(type);
            ValidateRegistrationBuilder(regBuilder, type, isSingleton: true);
        }

        [Test]
        public void Builder_Registers_Generics()
        {
            RegistrationBuilder regBuilder = _builder.Register<IInterface>();
            ValidateRegistrationBuilder(regBuilder, typeof(IInterface), isSingleton: false);
        }

        [Test]
        public void Builder_Registers_Generics_AsSingleton()
        {
            RegistrationBuilder regBuilder = _builder.RegisterSingleton<Class>();
            ValidateRegistrationBuilder(regBuilder, typeof(Class), isSingleton: true);
        }

        private void ValidateRegistrationBuilder(RegistrationBuilder regBuilder, Type expectedType, bool isSingleton)
        {
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedType, regBuilder.Type);
                Assert.AreEqual(isSingleton, regBuilder.IsSingleton);
                Assert.IsEmpty(regBuilder.Registrations);
            });
        }

        private static IEnumerable<Type> GetTypes()
        {
            yield return typeof(IInterface);
            yield return typeof(Class);
            yield return typeof(Struct);
        }

        private interface IInterface { }

        private class Class { }

        private struct Struct { }
    }
}