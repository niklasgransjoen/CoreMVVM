using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveTests : LifetimeScopeTestBase
    {
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
    }
}