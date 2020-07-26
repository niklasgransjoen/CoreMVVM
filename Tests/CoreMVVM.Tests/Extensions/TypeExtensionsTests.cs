using System;
using System.Collections.Generic;
using Xunit;

namespace CoreMVVM.Extensions.Tests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void TypeExtensions_GetDefault_Handles_ValueTypes()
        {
            Assert.Equal(0, typeof(int).GetDefault());
        }

        [Fact]
        public void TypeExtensions_GetDefault_Handles_ReferenceTypes()
        {
            Assert.Null(typeof(string).GetDefault());
        }

        [Fact]
        public void TypeExtensions_GetDefault_Handles_Interfaces()
        {
            Assert.Null(typeof(IDisposable).GetDefault());
        }

        #region TypeExtensions_IsAssignableFromGeneric_Test

        [Theory]
        [MemberData(nameof(GetIsAssignableFromGenericData))]
        public void TypeExtensions_IsAssignableFromGeneric_Test(Type t1, Type t2, bool implementsGenericType)
        {
            bool result = t1.IsAssignableFromGeneric(t2);
            Assert.Equal(implementsGenericType, result);
        }

        public static IEnumerable<object[]> GetIsAssignableFromGenericData()
        {
            foreach (var (t1, t2, result) in getData())
            {
                yield return new object[] { t1, t2, result };
            }

            static IEnumerable<(Type t1, Type t2, bool result)> getData()
            {
                // IService
                yield return (t1: typeof(IService), t2: typeof(ServiceA<>), result: true);
                yield return (t1: typeof(IService), t2: typeof(NonService), result: false);
                yield return (t1: typeof(IService<>), t2: typeof(ServiceA<>), result: true);
                yield return (t1: typeof(IService<>), t2: typeof(NonService), result: false);

                // IServiceA
                yield return (t1: typeof(IServiceA<>), t2: typeof(ServiceA<>), result: true);
                yield return (t1: typeof(IServiceA<int>), t2: typeof(ServiceA<>), result: false);
                yield return (t1: typeof(IServiceA<>), t2: typeof(ServiceA<int>), result: false);
                yield return (t1: typeof(IServiceA<int>), t2: typeof(ServiceA<int>), result: true);

                yield return (t1: typeof(IServiceA<>), t2: typeof(ServiceB<>), result: false);
                yield return (t1: typeof(IServiceA<int>), t2: typeof(ServiceB<>), result: true);
                yield return (t1: typeof(IServiceA<>), t2: typeof(ServiceB<int>), result: false);
                yield return (t1: typeof(IServiceA<int>), t2: typeof(ServiceB<int>), result: true);

                yield return (t1: typeof(IServiceA<>), t2: typeof(ServiceC), result: false);
                yield return (t1: typeof(IServiceA<int>), t2: typeof(ServiceC), result: true);

                // IServiceB
                yield return (t1: typeof(IServiceB<,>), t2: typeof(ServiceA<>), result: true);
                yield return (t1: typeof(IServiceB<int, int>), t2: typeof(ServiceA<>), result: false);
                yield return (t1: typeof(IServiceB<int, string>), t2: typeof(ServiceA<>), result: false);
                yield return (t1: typeof(IServiceB<string, int>), t2: typeof(ServiceA<>), result: false);
                yield return (t1: typeof(IServiceB<string, string>), t2: typeof(ServiceA<>), result: false);
                yield return (t1: typeof(IServiceB<,>), t2: typeof(ServiceA<int>), result: false);
                yield return (t1: typeof(IServiceB<int, int>), t2: typeof(ServiceA<int>), result: true);
                yield return (t1: typeof(IServiceB<int, string>), t2: typeof(ServiceA<int>), result: false);
                yield return (t1: typeof(IServiceB<string, int>), t2: typeof(ServiceA<int>), result: false);
                yield return (t1: typeof(IServiceB<string, string>), t2: typeof(ServiceA<int>), result: false);

                yield return (t1: typeof(IServiceB<,>), t2: typeof(ServiceB<>), result: false);
                yield return (t1: typeof(IServiceB<int, int>), t2: typeof(ServiceB<>), result: false);
                yield return (t1: typeof(IServiceB<int, string>), t2: typeof(ServiceB<>), result: false);
                yield return (t1: typeof(IServiceB<string, int>), t2: typeof(ServiceB<>), result: false);
                yield return (t1: typeof(IServiceB<string, string>), t2: typeof(ServiceB<>), result: false);
                yield return (t1: typeof(IServiceB<,>), t2: typeof(ServiceB<int>), result: false);
                yield return (t1: typeof(IServiceB<int, int>), t2: typeof(ServiceB<int>), result: true);
                yield return (t1: typeof(IServiceB<int, string>), t2: typeof(ServiceB<int>), result: false);
                yield return (t1: typeof(IServiceB<string, int>), t2: typeof(ServiceB<int>), result: false);
                yield return (t1: typeof(IServiceB<string, string>), t2: typeof(ServiceB<int>), result: false);
                yield return (t1: typeof(IServiceB<int, int>), t2: typeof(ServiceB<string>), result: false);
                yield return (t1: typeof(IServiceB<int, string>), t2: typeof(ServiceB<string>), result: true);
                yield return (t1: typeof(IServiceB<string, int>), t2: typeof(ServiceB<string>), result: false);
                yield return (t1: typeof(IServiceB<string, string>), t2: typeof(ServiceB<string>), result: false);

                yield return (t1: typeof(IServiceB<,>), t2: typeof(ServiceC), result: false);
                yield return (t1: typeof(IServiceB<int, int>), t2: typeof(ServiceC), result: true);
                yield return (t1: typeof(IServiceB<int, string>), t2: typeof(ServiceC), result: false);
                yield return (t1: typeof(IServiceB<string, int>), t2: typeof(ServiceC), result: false);
                yield return (t1: typeof(IServiceB<string, string>), t2: typeof(ServiceC), result: false);

                // ServiceD
                yield return (t1: typeof(ServiceB<>), t2: typeof(ServiceD<,>), result: true);
                yield return (t1: typeof(ServiceB<int>), t2: typeof(ServiceD<,>), result: false);
                yield return (t1: typeof(ServiceB<>), t2: typeof(ServiceD<int, int>), result: false);
                yield return (t1: typeof(ServiceB<>), t2: typeof(ServiceD<int, string>), result: false);
                yield return (t1: typeof(ServiceB<>), t2: typeof(ServiceD<string, int>), result: false);
                yield return (t1: typeof(ServiceB<>), t2: typeof(ServiceD<string, string>), result: false);
                yield return (t1: typeof(ServiceB<int>), t2: typeof(ServiceD<int, int>), result: true);
                yield return (t1: typeof(ServiceB<int>), t2: typeof(ServiceD<int, string>), result: true);
                yield return (t1: typeof(ServiceB<int>), t2: typeof(ServiceD<string, int>), result: false);
                yield return (t1: typeof(ServiceB<int>), t2: typeof(ServiceD<string, string>), result: false);

                // ServiceE
                yield return (t1: typeof(ServiceB<>), t2: typeof(ServiceE<>), result: false);
                yield return (t1: typeof(ServiceB<int>), t2: typeof(ServiceE<>), result: true);
                yield return (t1: typeof(ServiceB<>), t2: typeof(ServiceE<int>), result: false);
                yield return (t1: typeof(ServiceB<int>), t2: typeof(ServiceE<int>), result: true);
            }
        }

        private interface IService
        {
        }

        private interface IService<T> : IService
        {
        }

        private interface IServiceA<T> : IService<T>
        {
        }

        private interface IServiceB<T1, T2> : IService<T1>
        {
        }

        private class NonService
        {
        }

        private class ServiceA<T> : IServiceA<T>, IServiceB<T, T>
        {
        }

        private class ServiceB<T> : IServiceA<int>, IServiceB<int, T>
        {
        }

        private class ServiceC : IServiceA<int>, IServiceB<int, int>
        {
        }

        private class ServiceD<T1, T2> : ServiceB<T1>
        {
        }

        private class ServiceE<T> : ServiceB<int>
        {
        }

        #endregion TypeExtensions_IsAssignableFromGeneric_Test
    }
}