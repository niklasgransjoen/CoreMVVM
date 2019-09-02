using CoreMVVM.Demo.ViewModels;
using CoreMVVM.Demo.Views;
using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using CoreMVVM.Windows;
using System.Windows;

namespace CoreMVVM.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WindowsApplication
    {
        protected override void OnStartupOverride(StartupEventArgs e)
        {
            base.OnStartup(e);

            Container.Resolve<IWindowManager>().ShowWindow<MainWindowModel>();
        }

        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterSingleton<MainWindow>();
            builder.RegisterSingleton<MainWindowModel>();

            builder.RegisterSingleton<ScreenPrinter>().As<ILogger>().AsSelf();

            builder.OnBuild += Builder_OnBuild;
        }

        private void Builder_OnBuild(IContainer container)
        {
            IViewLocator viewLocator = container.Resolve<IViewLocator>();

            viewLocator.RegisterView<DialogWindowModel, DialogWindow>();
        }
    }
}