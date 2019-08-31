using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using NUnit.Framework;

namespace CoreMVVM.Tests.IOC
{
    [TestFixture]
    public class OwnedTests
    {
        [Test]
        public void Owned_Downcasts()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Register<Implementation>().As<IInterface>();
            IContainer container = builder.Build();

            IOwned<IInterface> instance = container.Resolve<IOwned<IInterface>>();
            Assert.DoesNotThrow(() =>
            {
                IOwned<object> downcastOwned = instance;
            });
        }
    }
}