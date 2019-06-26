using CoreMVVM.Demo.ViewModels;
using CoreMVVM.Demo.Views;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            container.Resolve<IWindowManager>().ShowWindow<MainWindowModel>();
        }

        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterSingleton<MainWindow>();
            builder.RegisterSingleton<MainWindowModel>();

            builder.RegisterSingleton<ScreenPrinter>().As<ILogger>().AsSelf();
        }
    }
}