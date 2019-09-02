using CoreMVVM.IOC.Builder;
using NUnit.Framework;
using System.Windows;

namespace CoreMVVM.Windows.Tests
{
    [TestFixture]
    public class WindowsApplicationTests
    {
        [Test]
        public void WindowsApplication_Disposes_Container()
        {
            TestApp app = new TestApp();

            Assert.IsFalse(app.ContainerIsDisposed);
            app.Run();
            Assert.IsTrue(app.ContainerIsDisposed);
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