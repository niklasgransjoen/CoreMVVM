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
        protected IContainer Container { get; private set; }

        protected WindowsApplication()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected sealed override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterSingleton<ViewLocator>().As<IViewLocator>();
            builder.RegisterSingleton<WindowManager>().As<IWindowManager>();

            RegisterComponents(builder);
            Container = builder.Build();

            OnStartupOverride(e);
        }

        protected sealed override void OnExit(ExitEventArgs e)
        {
            OnExitOverride(e);

            Container.Dispose();

            base.OnExit(e);
        }

        protected abstract void RegisterComponents(ContainerBuilder builder);

        protected virtual void OnStartupOverride(StartupEventArgs e)
        {
        }

        protected virtual void OnExitOverride(ExitEventArgs e)
        {
        }

        #region Listeners

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ILogger logger = Container.Resolve<ILogger>();
            logger.Exception("UnhandledException", e.ExceptionObject as Exception);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ILogger logger = Container.Resolve<ILogger>();
            logger.Exception("DispatcherUnhandledException", e.Exception);
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ILogger logger = Container.Resolve<ILogger>();
            logger.Exception("UnobservedTaskException", e.Exception);
        }

        #endregion Listeners
    }
}