using System;

namespace CoreMVVM.Input
{
    public class RelayCommand<T> : ICommandExt
    {
        /// <summary>
        /// Gets a reference to an empty relay command.
        /// </summary>
        public static RelayCommand<T> Empty { get; } = new RelayCommand<T>(() => { });

        #region Fields

        /// <summary>
        /// Local copy of <see cref="RelayCommand.CanExecuteChangedSubscriptionForwarder"/>, so that we subscribe and unsubscribe from the same instance.
        /// </summary>
        private readonly ICommandCanExecuteChangedSubscriptionForwarder? _subscriptionForwarder = RelayCommand.CanExecuteChangedSubscriptionForwarder;

        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute = null;

        private event EventHandler? _canExecuteChanged;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <exception cref="ArgumentNullException">execute is null.</exception>
        public RelayCommand(Action execute)
        {
            if (execute is null)
                throw new ArgumentNullException(nameof(execute));

            _execute = param => execute();
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <exception cref="ArgumentNullException">execute is null.</exception>
        public RelayCommand(Action<T> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="ArgumentNullException">execute or canExecute is null.</exception>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            if (canExecute is null) throw new ArgumentNullException(nameof(canExecute));

            _execute = param => execute();
            _canExecute = param => canExecute();
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="ArgumentNullException">execute or canExecute is null.</exception>
        public RelayCommand(Action execute, Func<T, bool> canExecute)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            if (canExecute is null) throw new ArgumentNullException(nameof(canExecute));

            _execute = param => execute();
            _canExecute = canExecute;
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="ArgumentNullException">execute or canExecute is null.</exception>
        public RelayCommand(Action<T> execute, Func<bool> canExecute)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            if (canExecute is null) throw new ArgumentNullException(nameof(canExecute));

            _execute = execute;
            _canExecute = param => canExecute();
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="ArgumentNullException">execute or canExecute is null.</exception>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));
            if (canExecute is null) throw new ArgumentNullException(nameof(canExecute));

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion Constructors

        #region ICommand Members

        ///<summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        ///</summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
                _subscriptionForwarder?.Subscribe(value);
            }
            remove
            {
                _canExecuteChanged -= value;
                _subscriptionForwarder?.Unsubscribe(value);
            }
        }

        /// <summary>
        /// Returns a value indicating if this command can be executed.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        ///<summary>
        /// Executes this command.
        ///</summary>
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        #endregion ICommand Members

        public void RaiseCanExecute()
        {
            var scheduler = RelayCommand.CanExecuteChangedScheduler;
            if (scheduler is null)
                RaiseCanExecute_Internal();
            else
                scheduler.Schedule(RaiseCanExecute_Internal);
        }

        private void RaiseCanExecute_Internal()
        {
            _canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}