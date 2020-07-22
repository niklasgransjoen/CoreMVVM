using CoreMVVM.Demo.ViewModels;
using CoreMVVM.Demo.Views;
using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using CoreMVVM.Windows;
using Microsoft.Extensions.Logging;
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
            Container.ResolveRequiredService<IWindowManager>()
                .ShowWindow<MainWindowModel>();
        }

        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterSingleton<ScreenPrinter>().As<ILogger>();
            builder.RegisterSingleton<ResourceManager>().As<IResourceService>();

            builder.OnBuild += OnContainerBuild;
        }

        private void OnContainerBuild(IContainer container)
        {
            ContainerProvider.Container = container;

            var viewProvider = new ViewProvider();
            viewProvider.RegisterView<DialogWindowModel, DialogWindow>();

            IViewLocator viewLocator = container.ResolveRequiredService<IViewLocator>();
            viewLocator.AddViewProvider(viewProvider);
        }
    }
}