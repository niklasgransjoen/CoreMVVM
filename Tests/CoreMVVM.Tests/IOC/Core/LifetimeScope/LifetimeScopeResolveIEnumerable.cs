using CoreMVVM.IOC.Builder;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveIEnumerable : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            LifetimeScope_Resolves_IEnumerable_Setup(builder);
            LifetimeScope_Resolves_IEnumerable_With_Duplicate_Registrations_Setup(builder);
        }

        #region LifetimeScope_Resolves_IEnumerable

        private void LifetimeScope_Resolves_IEnumerable_Setup(ContainerBuilder builder)
        {
            builder.RegisterTransient<Service1>().As<IServiceA>();
            builder.RegisterTransient<Service2>().As<IServiceA>();
        }

        [Fact]
        public void LifetimeScope_Resolves_IEnumerable()
        {
            Assert.Collection(LifetimeScope.ResolveRequiredService<IEnumerable<IServiceA>>(),
                new Action<IServiceA>[]
                {
                    service => Assert.IsType<Service1>(service),
                    service => Assert.IsType<Service2>(service),
                });
        }

        private interface IServiceA
        {
        }

        private class Service1 : IServiceA
        {
        }

        private class Service2 : IServiceA
        {
        }

        #endregion LifetimeScope_Resolves_IEnumerable

        #region LifetimeScope_Resolves_IEnumerable_With_Duplicate_Registrations

        private void LifetimeScope_Resolves_IEnumerable_With_Duplicate_Registrations_Setup(ContainerBuilder builder)
        {
            builder.RegisterTransient<Service3>().As<IServiceB>();
            builder.RegisterTransient<Service3>().As<IServiceB>();
            builder.RegisterSingleton<Service3>().As<IServiceB>();
        }

        [Fact]
        public void LifetimeScope_Resolves_IEnumerable_With_Duplicate_Registrations()
        {
            Assert.Collection(LifetimeScope.ResolveServices<IServiceB>(),
                new Action<IServiceB>[]
                {
                    service => Assert.IsType<Service3>(service),
                    service => Assert.IsType<Service3>(service),
                    service => Assert.IsType<Service3>(service),
                });
        }

        private interface IServiceB
        {
        }

        private class Service3 : IServiceB
        {
        }

        #endregion LifetimeScope_Resolves_IEnumerable_With_Duplicate_Registrations
    }
}