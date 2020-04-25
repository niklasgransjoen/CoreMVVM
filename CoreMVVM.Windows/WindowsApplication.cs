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
        private IContainer _container;

        protected IContainer Container => _container ?? throw new NotInitializedException();

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

            builder.RegisterSingleton<WindowsViewLocator>().As<IViewLocator>();

            RegisterComponents(builder);
            builder.OnBuild += OnContainerBuilt;
            _container = builder.Build();

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

        private void OnContainerBuilt(IContainer container)
        {
            ContainerProvider.Container = container;
        }

        #region UnhandledException

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                LoggerHelper.Exception("UnhandledException", exception);
            else
                LoggerHelper.Error($"Unhandled exception: {e.ExceptionObject}.");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LoggerHelper.Exception("DispatcherUnhandledException", e.Exception);
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LoggerHelper.Exception("UnobservedTaskException", e.Exception);
        }

        #endregion UnhandledException

        #endregion Listeners
    }
}