using CoreMVVM.Tests;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoreMVVM.IOC.Builder.Tests
{
    public class ContainerBuilderTests
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();

        #region No scope

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Builder_Registers(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterTransient(type);
            ValidateRegistrationBuilder(regBuilder, type, ComponentScope.Transient);
        }

        [Fact]
        public void Builder_Registers_Generics()
        {
            IRegistrationBuilder regBuilder = _builder.RegisterTransient<IInterface>();
            ValidateRegistrationBuilder(regBuilder, typeof(IInterface), ComponentScope.Transient);
        }

        [Theory]
        [MemberData(nameof(GetFactories))]
        public void Builder_Registers_Factories(Func<ILifetimeScope, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterTransient(factory);
            ValidateRegistrationBuilder(regBuilder, typeof(object), ComponentScope.Transient);
        }

        #endregion No scope

        #region Singleton

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Builder_Registers_Singleton(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterSingleton(type);
            ValidateRegistrationBuilder(regBuilder, type, ComponentScope.Singleton);
        }

        [Fact]
        public void Builder_Registers_GenericSingleton()
        {
            IRegistrationBuilder regBuilder = _builder.RegisterSingleton<Class>();
            ValidateRegistrationBuilder(regBuilder, typeof(Class), ComponentScope.Singleton);
        }

        [Theory]
        [MemberData(nameof(GetFactories))]
        public void Builder_Registers_Singleton_Factories(Func<ILifetimeScope, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterSingleton(factory);
            ValidateRegistrationBuilder(regBuilder, typeof(object), ComponentScope.Singleton);
        }

        #endregion Singleton

        #region Lifetime scope

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Builder_Registers_LifetimeScope(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterLifetimeScope(type);
            ValidateRegistrationBuilder(regBuilder, type, ComponentScope.LifetimeScope);
        }

        [Fact]
        public void Builder_Registers_GenericLifetimeScope()
        {
            IRegistrationBuilder regBuilder = _builder.RegisterLifetimeScope<Class>();
            ValidateRegistrationBuilder(regBuilder, typeof(Class), ComponentScope.LifetimeScope);
        }

        [Theory]
        [MemberData(nameof(GetFactories))]
        public void Builder_Registers_LifetimeScope_Factories(Func<ILifetimeScope, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.RegisterLifetimeScope(factory);
            ValidateRegistrationBuilder(regBuilder, typeof(object), ComponentScope.LifetimeScope);
        }

        #endregion Lifetime scope

        [Fact]
        public void Builder_ThrowsOn_ConflictingTypes()
        {
            IRegistrationBuilder regBuilder = _builder.RegisterTransient<Class>();

            Assert.Throws<IncompatibleTypeException>(() => regBuilder.As<IInterface>());
        }

        #region Helpers

        private static void ValidateRegistrationBuilder(IRegistrationBuilder regBuilder, Type expectedType, ComponentScope scope)
        {
            Assert.Equal(expectedType, regBuilder.Type);
            Assert.Equal(scope, regBuilder.Scope);
        }

        #endregion Helpers

        #region Test data

        public static IEnumerable<object[]> GetTypes()
        {
            foreach (var type in getData())
                yield return new object[] { type };

            static IEnumerable<Type> getData()
            {
                yield return typeof(IInterface);
                yield return typeof(Class);
                yield return typeof(Struct);
            }
        }

        public static IEnumerable<object[]> GetFactories()
        {
            foreach (var factory in getData())
                yield return new object[] { factory };

            static IEnumerable<Func<ILifetimeScope, object>> getData()
            {
                yield return c => new Class();
                yield return c => new Struct();
                yield return c => new Implementation();
            }
        }

        #endregion Test data
    }
}