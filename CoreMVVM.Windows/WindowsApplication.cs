using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

#if !NET45

using Microsoft.Extensions.Logging;

#endif

namespace CoreMVVM.Windows
{
    public abstract class WindowsApplication : Application
    {
        private IContainer _container;

        protected IContainer Container => _container ?? throw new NotInitializedException();

        protected WindowsApplication()
        {
#if !NET45
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
#endif
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

#if !NET45

        #region UnhandledException

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = Container.ResolveService<ILogger<WindowsApplication>>();
            if (logger is null)
                return;

            if (e.ExceptionObject is Exception exception)
                logger.LogError(exception, "Unhandled Exception");
            else
                logger.LogError($"Unhandled exception: {e.ExceptionObject}.");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = Container.ResolveService<ILogger<WindowsApplication>>();
            if (logger is null)
                return;

            logger.LogError(e.Exception, "DispatcherUnhandledException", e.Exception);
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var logger = Container.ResolveService<ILogger<WindowsApplication>>();
            if (logger is null)
                return;

            logger.LogError(e.Exception, "UnobservedTaskException");
        }

        #endregion UnhandledException

#endif

        #endregion Listeners
    }
}