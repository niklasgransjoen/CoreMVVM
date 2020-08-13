using System;

namespace CoreMVVM.Input
{
    /// <summary>
    /// Service for scheduling invoke of <see cref="System.Windows.Input.ICommand.CanExecuteChanged"/>.
    /// </summary>
    /// <remarks>
    /// This service can be implemented to i.e. force the <see cref="System.Windows.Input.ICommand.CanExecuteChanged"/> event
    /// to be invoked on the UI thread.
    /// </remarks>
    public interface ICommandCanExecuteChangedScheduler
    {
        /// <summary>
        /// Schedules invoke of action.
        /// </summary>
        void Schedule(Action canExecuteChangedAction);
    }
}