using System;
using System.Collections.Generic;
using Xunit;

namespace CoreMVVM.IOC.Builder.Tests
{
    public class RegistrationBuilderTests
    {
        #region RegistrationBuilder_OverridesOnDuplicate

        [Fact]
        public void RegistrationBuilder_OverridesOnDuplicate()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterTransient<Impl1>().As<ISimple>();
            builder.RegisterTransient<Impl2>().As<ISimple>();
            IContainer container = builder.Build();

            var instance = container.ResolveRequiredService<ISimple>();
            Assert.IsType<Impl2>(instance);
        }

        private interface ISimple { }

        private class Impl1 : ISimple { }

        private class Impl2 : ISimple { }

        #endregion RegistrationBuilder_OverridesOnDuplicate

        #region RegistrationBuilder_Handles_Valid_Generic_Registrations

        [Theory]
        [MemberData(nameof(GetValidGenericRegistrationData))]
        public void RegistrationBuilder_Handles_Valid_Generic_Registrations(Type serviceType, Type implementationType, Type[] typeArguments)
        {
            var builder = new ContainerBuilder();

            builder.RegisterTransient(implementationType).As(serviceType);

            var container = builder.Build();

            var service = container.ResolveRequiredService(serviceType.MakeGenericType(typeArguments));
            Assert.IsType(implementationType.MakeGenericType(typeArguments), service);
        }

        public static IEnumerable<object[]> GetValidGenericRegistrationData()
        {
            foreach (var (serviceType, implementationType, typeArguments) in getData())
            {
                yield return new object[] { serviceType, implementationType, typeArguments };
            }

            static IEnumerable<(Type serviceType, Type implementationType, Type[] typeArguments)> getData()
            {
                yield return (typeof(IService1<>), typeof(Service1<>), new[] { typeof(int) });
                yield return (typeof(IService2<,>), typeof(Service2<,>), new[] { typeof(int), typeof(int) });
                yield return (typeof(IService3<,,>), typeof(Service3<,,>), new[] { typeof(int), typeof(int), typeof(string) });
                yield return (typeof(IService4<,>), typeof(Service4<,>), new[] { typeof(int), typeof(Service1<int>) });
            }
        }

        private interface IService1<T>
        {
        }

        private class Service1<T> : IService1<T>
        {
        }

        private interface IService2<T1, T2>
        {
        }

        private class Service2<T1, T2> : IService2<T1, T2>
        {
        }

        private interface IService3<T1, T2, T3>
            where T1 : new()
            where T3 : class
        {
        }

        private class Service3<T1, T2, T3> : IService3<T1, T2, T3>
            where T1 : new()
            where T3 : class
        {
        }

        private interface IService4<T1, T2>
            where T2 : new()
        {
        }

        private class Service4<T2, T1> : IService4<T2, T1>
            where T1 : class, new()
            where T2 : unmanaged
        {
        }

        #endregion RegistrationBuilder_Handles_Valid_Generic_Registrations

        #region RegistrationBuilder_Throws_On_Invalid_Generic_Registrations

        [Theory]
        [MemberData(nameof(GetInvalidGenericRegistrationData))]
        public void RegistrationBuilder_Throws_On_Invalid_Generic_Registrations(Type serviceType, Type implementationType)
        {
            var builder = new ContainerBuilder();
            Assert.Throws<IncompatibleGenericTypeDefinitionException>(() => builder.RegisterTransient(implementationType).As(serviceType));
        }

        public static IEnumerable<object[]> GetInvalidGenericRegistrationData()
        {
            foreach (var (serviceType, implementationType) in getData())
            {
                yield return new object[] { serviceType, implementationType };
            }

            static IEnumerable<(Type serviceType, Type implementationType)> getData()
            {
                yield return (typeof(IServiceA<>), typeof(ServiceA<,>));
                yield return (typeof(IServiceB<,>), typeof(ServiceB<>));
                yield return (typeof(IServiceC<,>), typeof(ServiceC<,>));
                yield return (typeof(IServiceD<,>), typeof(ServiceD<,>));
            }
        }

        private interface IServiceA<T>
        {
        }

        private class ServiceA<T1, T2> : IServiceA<T1>
        {
        }

        private interface IServiceB<T1, T2>
        {
        }

        private class ServiceB<T> : IServiceB<T, T>
        {
        }

        private interface IServiceC<T1, T2>
        {
        }

        private class ServiceC<T1, T2> : IServiceC<T2, T1>
        {
        }

        private interface IServiceD<T1, T2>
        {
        }

        private class ServiceD<T2, T1> : IServiceD<T1, T2>
        {
        }

        #endregion RegistrationBuilder_Throws_On_Invalid_Generic_Registrations
    }
}