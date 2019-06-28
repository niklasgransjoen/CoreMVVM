using CoreMVVM.IOC.Builder;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CoreMVVM.Tests.IOC.Builder
{
    [TestFixture]
    public class ContainerBuilderTests
    {
        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Types(Type type)
        {
            ContainerBuilder builder = new ContainerBuilder(registerDefaults: false);
            RegistrationBuilder regBuilder = builder.Register(type);

            Assert.AreEqual(type, regBuilder.Type);
            Assert.IsFalse(regBuilder.IsSingleton);
            Assert.IsEmpty(regBuilder.Registrations);
        }

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Types_AsSingletons(Type type)
        {
            ContainerBuilder builder = new ContainerBuilder(registerDefaults: false);
            RegistrationBuilder regBuilder = builder.RegisterSingleton(type);

            Assert.AreEqual(type, regBuilder.Type);
            Assert.IsTrue(regBuilder.IsSingleton);
            Assert.IsEmpty(regBuilder.Registrations);
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