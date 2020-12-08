using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using Xunit;

namespace CoreMVVM.IOC.Tests
{
    public class OwnedTests
    {
        [Fact]
        public void Owned_Downcasts()
        {
            var builder = new ContainerBuilder();
            builder.RegisterTransient<Implementation>().As<IInterface>();
            var container = builder.Build();

            var instance = container.ResolveRequiredService<IOwned<IInterface>>();
            var e = Record.Exception(() =>
            {
                IOwned<object> downcastOwned = instance;
            });
            Assert.Null(e);
        }
    }
}