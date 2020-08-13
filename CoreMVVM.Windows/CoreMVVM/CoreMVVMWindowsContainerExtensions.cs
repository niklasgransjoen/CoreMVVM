using CoreMVVM.Input;
using CoreMVVM.Windows.Input;
using System;

namespace CoreMVVM.IOC
{
    public static class CoreMVVMWindowsContainerExtensions
    {
        /// <summary>
        /// Configures CoreMVVM.Windows' services.
        /// </summary>
        public static IContainer ConfigureWindowsServices(this IContainer container)
        {
            if (container is null)
                throw new ArgumentNullException(nameof(container));

            RelayCommand.CanExecuteChangedScheduler = container.ResolveRequiredService<ICommandCanExecuteChangedUIThreader>();
            RelayCommand.CanExecuteChangedSubscriptionForwarder = container.ResolveRequiredService<ICommandCanExecuteChangedWindowsSubscriptionForwarder>();

            return container;
        }
    }
}