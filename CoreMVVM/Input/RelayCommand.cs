using System;
using System.Windows.Input;

namespace CoreMVVM.Input
{
    /// <summary>
    /// A default implementation of <see cref="ICommand"/>.
    /// </summary>
    public class RelayCommand : ICommandExt
    {
        /// <summary>
        /// Gets or sets a custom service for sceduling invocations of <see cref="CanExecuteChanged"/>.
        /// </summary>
        /// <remarks>This is used by both <see cref="RelayCommand"/> and <see cref="RelayCommand{T}"/>.</remarks>
        public static ICommandCanExecuteChangedScheduler? CanExecuteChangedScheduler { get; set; }

        /// <summary>
        /// Gets or sets a custom service for forwarding subscriptions to <see cref="CanExecuteChanged"/>.
        /// </summary>
        /// <remarks>This is used by both <see cref="RelayCommand"/> and <see cref="RelayCommand{T}"/>.</remarks>
        public static ICommandCanExecuteChangedSubscriptionForwarder? CanExecuteChangedSubscriptionForwarder { get; set; }

        /// <summary>
        /// Gets a reference to an empty relay command.
        /// </summary>
        public static RelayCommand Empty { get; } = new RelayCommand(() => { });

        #region Fields

        /// <summary>
        /// Local copy of <see cref="CanExecuteChangedSubscriptionForwarder"/>, so that we subscribe and unsubscribe from the same instance.
        /// </summary>
        private readonly ICommandCanExecuteChangedSubscriptionForwarder? _subscriptionForwarder = CanExecuteChangedSubscriptionForwarder;

        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute = null;

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
        public RelayCommand(Action<object?> execute)
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
        public RelayCommand(Action execute, Func<object?, bool> canExecute)
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
        public RelayCommand(Action<object?> execute, Func<bool> canExecute)
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
        public RelayCommand(Action<object?> execute, Func<object?, bool> canExecute)
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
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        #endregion ICommand Members

        public void RaiseCanExecute()
        {
            var scheduler = CanExecuteChangedScheduler;
            if (scheduler is null)
                invoke();
            else
                scheduler.Schedule(invoke);

            void invoke()
            {
                _canExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}