using CoreMVVM.IOC.Builder;
using System.Windows;
using Xunit;

namespace CoreMVVM.Windows.Tests
{
    public class WindowsApplicationTests
    {
        [Fact]
        public void WindowsApplication_Disposes_Container()
        {
            TestApp app = new TestApp();

            Assert.False(app.ContainerIsDisposed);
            app.Run();
            Assert.True(app.ContainerIsDisposed);
        }

        private class TestApp : WindowsApplication
        {
            public bool ContainerIsDisposed => Container?.IsDisposed ?? false;

            protected override void OnStartupOverride(StartupEventArgs e)
            {
                Shutdown();
            }

            protected override void RegisterComponents(ContainerBuilder builder)
            {
            }
        }
    }
}