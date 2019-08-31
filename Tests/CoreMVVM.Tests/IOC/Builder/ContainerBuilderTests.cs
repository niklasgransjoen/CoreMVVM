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

        #region No scope

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.Register(type);
            ValidateRegistrationBuilder(regBuilder, type, InstanceScope.None);
        }

        [Test]
        public void Builder_Registers_Generics()
        {
            IRegistrationBuilder regBuilder = _builder.Register<IInterface>();
            ValidateRegistrationBuilder(regBuilder, typeof(IInterface), InstanceScope.None);
        }

        [TestCaseSource(nameof(GetFactories))]
        public void Builder_Registers_Factories(Func<ILifetimeScope, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.Register(factory);
            ValidateRegistrationBuilder(regBuilder, typeof(object), InstanceScope.None);
        }

        #endregion No scope

        #region Singleton

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Singleton(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterSingleton(type);
            ValidateRegistrationBuilder(regBuilder, type, InstanceScope.Singleton);
        }

        [Test]
        public void Builder_Registers_GenericSingleton()
        {
            IRegistrationBuilder regBuilder = _builder.RegisterSingleton<Class>();
            ValidateRegistrationBuilder(regBuilder, typeof(Class), InstanceScope.Singleton);
        }

        [TestCaseSource(nameof(GetFactories))]
        public void Builder_Registers_Singleton_Factories(Func<ILifetimeScope, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterSingleton(factory);
            ValidateRegistrationBuilder(regBuilder, typeof(object), InstanceScope.Singleton);
        }

        #endregion Singleton

        #region Lifetime scope

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_LifetimeScope(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterLifetimeScope(type);
            ValidateRegistrationBuilder(regBuilder, type, InstanceScope.LifetimeScope);
        }

        [Test]
        public void Builder_Registers_GenericLifetimeScope()
        {
            IRegistrationBuilder regBuilder = _builder.RegisterLifetimeScope<Class>();
            ValidateRegistrationBuilder(regBuilder, typeof(Class), InstanceScope.LifetimeScope);
        }

        [TestCaseSource(nameof(GetFactories))]
        public void Builder_Registers_LifetimeScope_Factories(Func<ILifetimeScope, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterLifetimeScope(factory);
            ValidateRegistrationBuilder(regBuilder, typeof(object), InstanceScope.LifetimeScope);
        }

        #endregion Lifetime scope

        [TestCaseSource(nameof(GetConflictingBuilders))]
        public void Builder_ThrowsOnConflictingScope((Action reg1, Action reg2) data)
        {
            data.reg1();
            Assert.Throws<ScopingConflictException>(() => data.reg2());
        }

        #region Helpers

        private void ValidateRegistrationBuilder(IRegistrationBuilder regBuilder, Type expectedType, InstanceScope scope)
        {
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedType, regBuilder.Type);
                Assert.AreEqual(scope, regBuilder.Scope);
            });
        }

        #endregion Helpers

        #region Test data

        private static IEnumerable<Type> GetTypes()
        {
            yield return typeof(IInterface);
            yield return typeof(Class);
            yield return typeof(Struct);
        }

        private static IEnumerable<Func<ILifetimeScope, object>> GetFactories()
        {
            yield return c => new Class();
            yield return c => new Struct();
            yield return c => new Implementation();
        }

        /// <summary>
        /// Returns registrators that registrate <see cref="IInterface"/> in different scopes.
        /// </summary>
        private static IEnumerable<(Action reg1, Action reg2)> GetConflictingBuilders()
        {
            foreach (var (builder1, scope1) in getRegistrators())
            {
                foreach (var (builder2, scope2) in getRegistrators())
                {
                    if (scope1 == scope2)
                        continue;

                    ContainerBuilder b = new ContainerBuilder();
                    yield return (() => builder1(b), () => builder2(b));
                }
            }

            IEnumerable<(Action<ContainerBuilder> builder, InstanceScope scope)> getRegistrators()
            {
                Type t = typeof(IInterface);

                yield return (b => b.Register(t).AsSelf(), InstanceScope.None);
                yield return (b => b.RegisterSingleton(t).AsSelf(), InstanceScope.Singleton);
                yield return (b => b.RegisterLifetimeScope(t).AsSelf(), InstanceScope.LifetimeScope);
            }
        }

        #endregion Test data
    }
}