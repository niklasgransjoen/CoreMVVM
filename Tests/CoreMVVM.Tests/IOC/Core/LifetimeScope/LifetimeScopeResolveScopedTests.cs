using CoreMVVM.IOC.Builder;
using CoreMVVM.Tests;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveScopedTests : LifetimeScopeTestBase
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterLifetimeScope<Implementation>().As<IInterface>().AsSelf();
        }

        [Fact]
        public void LifetimeScope_ResolvesFromRoot_ToSingleInstance()
        {
            object subject1 = LifetimeScope.ResolveRequiredService<IInterface>();
            object subject2 = LifetimeScope.ResolveRequiredService<IInterface>();

            Assert.Equal(subject1, subject2);
        }

        [Fact]
        public void LifetimeScope_ResolvesFromSubscope_ToSingleInstance()
        {
            using var subscope = LifetimeScope.BeginLifetimeScope();

            object subject1 = subscope.ResolveRequiredService<IInterface>();
            object subject2 = subscope.ResolveRequiredService<IInterface>();

            Assert.Equal(subject1, subject2);
        }

        [Fact]
        public void LifetimeScope_Resolves_UniqueInstance_FromDifferentScopes()
        {
            using var subscope = LifetimeScope.BeginLifetimeScope();

            object subject1 = LifetimeScope.ResolveRequiredService<IInterface>();
            object subject2 = subscope.ResolveRequiredService<IInterface>();

            Assert.NotEqual(subject1, subject2);
        }

        [Fact]
        public async Task LifetimeScope_ResolveSingleton_ThreadSafe()
        {
            var subscope = LifetimeScope.BeginLifetimeScope();
            var resolvingTasks = new List<Task<IInterface>>
            {
                Task.Run(() => subscope.ResolveRequiredService<IInterface>()),
                Task.Run(() => subscope.ResolveRequiredService<IInterface>()),
                Task.Run(() => subscope.ResolveRequiredService<IInterface>()),
                Task.Run(() => subscope.ResolveRequiredService<IInterface>()),
                Task.Run(() => subscope.ResolveRequiredService<IInterface>()),
                Task.Run(() => subscope.ResolveRequiredService<IInterface>()),
                Task.Run(() => subscope.ResolveRequiredService<IInterface>()),
            };

            var interfaces = new List<IInterface>();
            foreach (var task in resolvingTasks)
                interfaces.Add(await task);

            while (interfaces.Count > 1)
            {
                for (var i = 1; i < interfaces.Count; i++)
                    Assert.Same(interfaces[0], interfaces[i]);

                interfaces.RemoveAt(0);
            }
        }
    }
}