using CoreMVVM.IOC.Builder;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            LifetimeScope_Handles_RegistrationOverrides_Setup(builder);
            LifetimeScope_Handles_Registrations_In_Different_Scopes_Setup(builder);
        }

        #region LifetimeScope_Resolve_Requires_Parameters

        [Fact]
        public void LifetimeScope_Resolve_Requires_Parameters()
        {
            Assert.Throws<ResolveUnregisteredServiceException>(() => LifetimeScope.ResolveService<MyClass>());
        }

        private class MyClass
        {
            public MyClass(IMyDependency dependency)
            {
                Dependency = dependency;
            }

            public IMyDependency Dependency { get; }
        }

        private interface IMyDependency
        {
        }

        #endregion LifetimeScope_Resolve_Requires_Parameters

        #region LifetimeScope_Handles_RegistrationOverrides

        private static void LifetimeScope_Handles_RegistrationOverrides_Setup(ContainerBuilder builder)
        {
            builder.RegisterSingleton<Service1>().As<IService1>();
            builder.RegisterSingleton<Service2>().As<IService2>();

            // Override IService2 registration.
            builder.RegisterSingleton<Service1>().As<IService2>();
        }

        [Fact]
        public void LifetimeScope_Handles_RegistrationOverrides()
        {
            var service1 = LifetimeScope.ResolveRequiredService<IService1>();
            var service2 = LifetimeScope.ResolveRequiredService<IService2>();

            Assert.Same(service1, service2);
        }

        private interface IService1
        {
        }

        private interface IService2
        {
        }

        private class Service1 : IService1, IService2
        {
        }

        private class Service2 : IService2
        {
        }

        #endregion LifetimeScope_Handles_RegistrationOverrides

        #region LifetimeScope_Handles_Registrations_In_Different_Scopes

        private static void LifetimeScope_Handles_Registrations_In_Different_Scopes_Setup(ContainerBuilder builder)
        {
            builder.RegisterSingleton<Service3>().As<IService3>();
            builder.RegisterLifetimeScope<Service3>().As<IService4>();
        }

        [Fact]
        public void LifetimeScope_Handles_Registrations_In_Different_Scopes()
        {
            var service1 = LifetimeScope.ResolveRequiredService<IService3>();
            var service2 = LifetimeScope.ResolveRequiredService<IService4>();

            Assert.NotSame(service1, service2);
        }

        private interface IService3
        {
        }

        private interface IService4
        {
        }

        private class Service3 : IService3, IService4
        {
        }

        #endregion LifetimeScope_Handles_Registrations_In_Different_Scopes
    }
}