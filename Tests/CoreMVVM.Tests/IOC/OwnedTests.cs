using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using System;
using Xunit;

namespace CoreMVVM.IOC.Tests
{
    public class OwnedTests
    {
        [Fact]
        public void Owned_Downcasts()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterTransient<Implementation>().As<IInterface>();
            IContainer container = builder.Build();

            IOwned<IInterface> instance = container.ResolveRequiredService<IOwned<IInterface>>();
            Exception e = Record.Exception(() =>
            {
                IOwned<object> downcastOwned = instance;
            });
            Assert.Null(e);
        }
    }
}