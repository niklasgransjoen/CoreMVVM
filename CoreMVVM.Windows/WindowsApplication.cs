using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CoreMVVM.Windows
{
    public abstract class WindowsApplication : Application
    {
        protected IContainer container;

        public WindowsApplication()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterSingleton<ViewLocator>().As<IViewLocator>();
            builder.RegisterSingleton<WindowManager>().As<IWindowManager>();

            RegisterComponents(builder);
            container = builder.Build();
        }

        protected abstract void RegisterComponents(ContainerBuilder builder);

        #region Listeners

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ILogger logger = container.Resolve<ILogger>();
            logger.Exception("UnhandledException", e.ExceptionObject as Exception);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ILogger logger = container.Resolve<ILogger>();
            logger.Exception("DispatcherUnhandledException", e.Exception);
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ILogger logger = container.Resolve<ILogger>();
            logger.Exception("UnobservedTaskException", e.Exception);
        }

        #endregion Listeners
    }
}