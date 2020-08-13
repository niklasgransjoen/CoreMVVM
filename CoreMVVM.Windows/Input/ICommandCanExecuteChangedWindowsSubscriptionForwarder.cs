using CoreMVVM.Input;
using System;
using System.Windows.Input;

namespace CoreMVVM.Windows.Input
{
    /// <summary>
    /// Implementation of <see cref="ICommandCanExecuteChangedSubscriptionForwarder"/> that subscribes handlers to <see cref="CommandManager.RequerySuggested"/>.
    /// </summary>
    public class ICommandCanExecuteChangedWindowsSubscriptionForwarder : ICommandCanExecuteChangedSubscriptionForwarder
    {
        public ICommandCanExecuteChangedWindowsSubscriptionForwarder()
        {
        }

        public virtual void Subscribe(EventHandler eventHandler)
        {
            CommandManager.RequerySuggested += eventHandler;
        }

        public virtual void Unsubscribe(EventHandler eventHandler)
        {
            CommandManager.RequerySuggested -= eventHandler;
        }
    }
}