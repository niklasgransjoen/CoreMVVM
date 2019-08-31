using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using NUnit.Framework;

namespace CoreMVVM.Tests.IOC.Builder
{
    [TestFixture]
    public class RegistrationBuilderTests
    {
        [Test]
        public void RegistrationBuilder_OverridesOnDuplicate()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Register<Impl1>().As<ISimple>();
            builder.Register<Impl2>().As<ISimple>();
            IContainer container = builder.Build();

            var instance = container.Resolve<ISimple>();
            Assert.AreEqual(typeof(Impl2), instance.GetType());
        }

        private interface ISimple { }

        private class Impl1 : ISimple { }

        private class Impl2 : ISimple { }
    }
}