using System;

namespace CoreMVVM.Input
{
    /// <summary>
    /// Service for forwarding subscriptions to <see cref="System.Windows.Input.ICommand.CanExecuteChanged"/>
    /// to custom event handlers.
    /// </summary>
    public interface ICommandCanExecuteChangedSubscriptionForwarder
    {
        void Subscribe(EventHandler? eventHandler);

        void Unsubscribe(EventHandler? eventHandler);
    }
}