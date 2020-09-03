using System;
using System.Windows.Input;

namespace CoreMVVM.Input
{
    /// <summary>
    /// A default generic implementation of <see cref="ICommand"/>.
    /// </summary>
    public class RelayCommand<T> : ICommand<T>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types

        /// <summary>
        /// Gets a reference to an empty relay command.
        /// </summary>
        public static RelayCommand<T> Empty { get; } = new RelayCommand<T>(() => { });

#pragma warning restore CA1000 // Do not declare static members on generic types

        #region Fields

        /// <summary>
        /// Local copy of <see cref="RelayCommand.CanExecuteChangedSubscriptionForwarder"/>, so that we subscribe and unsubscribe from the same instance.
        /// </summary>
        private readonly ICommandCanExecuteChangedSubscriptionForwarder? _subscriptionForwarder = RelayCommand.CanExecuteChangedSubscriptionForwarder;

        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

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
        ///<remarks>If <paramref name="parameter"/> is null, default(<typeparamref name="T"/>) is passed as the command's argument.</remarks>
        bool ICommand.CanExecute(object? parameter)
        {
            T castParam = ProcessParameter(parameter);
            return CanExecute(castParam);
        }

        /// <summary>
        /// Returns a value indicating if this command can be executed.
        /// </summary>
        public bool CanExecute(T parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        ///<summary>
        /// Executes this command.
        ///</summary>
        ///<remarks>If <paramref name="parameter"/> is null, default(<typeparamref name="T"/>) is passed as the command's argument.</remarks>
        void ICommand.Execute(object? parameter)
        {
            T castParam = ProcessParameter(parameter);
            Execute(castParam);
        }

        ///<summary>
        /// Executes this command.
        ///</summary>
        public void Execute(T parameter)
        {
            _execute(parameter);
        }

        #endregion ICommand Members

        public void RaiseCanExecute()
        {
            var scheduler = RelayCommand.CanExecuteChangedScheduler;
            if (scheduler is null)
                invoke();
            else
                scheduler.Schedule(invoke);

            void invoke()
            {
                _canExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #region Utilities

        /// <summary>
        /// Processes a command parameter.
        /// </summary>
        /// <param name="parameter">The parameter to process.</param>
        /// <returns>The cast parameter, or default(<typeparamref name="T"/>) if <paramref name="parameter"/> is null.</returns>
        /// <exception cref="ArgumentException">parameter is not null and of the wrong type.</exception>
        protected static T ProcessParameter(object? parameter) => parameter switch
        {
            T castParam => castParam,
            null => default!,

            _ => throw new ArgumentException($"Expected parameter to be of type '{typeof(T)}', was '{parameter.GetType()}'.", nameof(parameter))
        };

        #endregion Utilities
    }
}