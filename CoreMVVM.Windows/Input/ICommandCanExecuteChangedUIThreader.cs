using CoreMVVM.Input;
using System;

namespace CoreMVVM.Windows.Input
{
    /// <summary>
    /// Implementation of <see cref="ICommandCanExecuteChangedScheduler"/> that schedules actions on the UI thread.
    /// </summary>
    public class ICommandCanExecuteChangedUIThreader : ICommandCanExecuteChangedScheduler
    {
        private readonly IUIThreader _uiThreader;

        public ICommandCanExecuteChangedUIThreader(IUIThreader uiThreader)
        {
            _uiThreader = uiThreader;
        }

        public virtual void Schedule(Action canExecuteChangedAction)
        {
            _uiThreader.Schedule(canExecuteChangedAction);
        }
    }
}