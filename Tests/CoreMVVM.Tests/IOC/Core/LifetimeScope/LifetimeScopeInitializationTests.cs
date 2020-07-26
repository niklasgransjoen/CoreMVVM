using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using System;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeInitializationTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterTransient<InitClass>().AsSelf();
            builder.RegisterTransient(c => new InitClass()).As<IInterface>();

            builder.RegisterSingleton<InitClass2>().As<IInit>();
        }

        [Fact]
        public void LifetimeScope_Initializes_Resolved_Component()
        {
            var instance = LifetimeScope.ResolveRequiredService<InitClass>();

            Assert.True(instance.IsInitialized);
        }

        [Fact]
        public void LifetimeScope_Initilizes_FuncResult()
        {
            var factory = LifetimeScope.ResolveRequiredService<Func<InitClass>>();
            var instance = factory();

            Assert.True(instance.IsInitialized);
        }

        [Fact]
        public void LifetimeScope_Initializes_FactoryResult()
        {
            var instance = (InitClass)LifetimeScope.ResolveRequiredService<IInterface>();

            Assert.True(instance.IsInitialized);
        }

        [Fact]
        public void LifetimeScope_RegistersSingleton_Before_Initializing()
        {
            LifetimeScope.ResolveRequiredService<IInit>();
        }

        [Fact]
        public void LifetimeScope_Initializes_NestedTypes_Once()
        {
            InitClass2 instance = (InitClass2)LifetimeScope.ResolveRequiredService<IInit>();

            Assert.Equal(1, instance.InitClass.InitializationCount);
        }

        private interface IInit : IComponent
        {
            bool IsInitialized { get; }
        }

        private class InitClass : IInit, IInterface
        {
            public void InitializeComponent(ILifetimeScope lifetimeScope)
            {
                IsInitialized = true;
                InitializationCount++;
            }

            public bool IsInitialized { get; set; }
            public int InitializationCount { get; private set; }
        }

        private class InitClass2 : IInit
        {
            public InitClass2(InitClass initClass)
            {
                InitClass = initClass;
            }

            public void InitializeComponent(ILifetimeScope lifetimeScope)
            {
                lifetimeScope.ResolveRequiredService<IInit>();
                IsInitialized = true;
            }

            public bool IsInitialized { get; set; }
            public InitClass InitClass { get; }
        }
    }
}