using Xunit;

namespace CoreMVVM.IOC.Builder.Tests
{
    public class RegistrationBuilderTests
    {
        [Fact]
        public void RegistrationBuilder_OverridesOnDuplicate()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Register<Impl1>().As<ISimple>();
            builder.Register<Impl2>().As<ISimple>();
            IContainer container = builder.Build();

            var instance = container.ResolveRequiredService<ISimple>();
            Assert.IsType<Impl2>(instance);
        }

        private interface ISimple { }

        private class Impl1 : ISimple { }

        private class Impl2 : ISimple { }
    }
}