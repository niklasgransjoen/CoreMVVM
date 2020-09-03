using System.Windows.Input;

namespace CoreMVVM.Input
{
    /// <summary>
    /// A generic extension of <see cref="ICommand"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of this command.</typeparam>
    public interface ICommand<T> : ICommandExt
    {
        /// <summary>
        /// Returns a value indicating if this command can be executed.
        /// </summary>
        bool CanExecute(T parameter);

        ///<summary>
        /// Executes this command.
        ///</summary>
        void Execute(T parameter);
    }
}