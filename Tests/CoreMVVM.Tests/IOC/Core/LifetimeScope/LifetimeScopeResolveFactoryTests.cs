using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveFactoryTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterTransient(c => new ClassWithProperties
            {
                MyVal = 4,
            }).As<IInterface>();

            builder.RegisterSingleton(c => new SingletonWithProperties()
            {
                Str = "unique-string",
            }).AsSelf();
        }

        [Fact]
        public void LifetimeScope_Resolves_Factory()
        {
            IInterface subject = LifetimeScope.ResolveRequiredService<IInterface>();

            Assert.IsType<ClassWithProperties>(subject);
            Assert.Equal(4, ((ClassWithProperties)subject).MyVal);
        }

        [Fact]
        public void LifetimeScope_Resolves_Singleton_FromFactory()
        {
            SingletonWithProperties subject1 = LifetimeScope.ResolveRequiredService<SingletonWithProperties>();
            SingletonWithProperties subject2 = LifetimeScope.ResolveRequiredService<SingletonWithProperties>();

            Assert.Equal(subject1, subject2);
            Assert.Equal("unique-string", subject1.Str);
        }

        private class ClassWithProperties : IInterface
        {
            public int MyVal { get; set; }
        }

        private class SingletonWithProperties
        {
            public string? Str { get; set; }
        }
    }
}