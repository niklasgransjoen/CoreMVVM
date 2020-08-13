using System.Windows.Input;

namespace CoreMVVM
{
    /// <summary>
    /// An extension of the <see cref="ICommand"/> interface.
    /// </summary>
    public interface ICommandExt : ICommand
    {
        /// <summary>
        /// Forces the <see cref="ICommand.CanExecuteChanged"/> event to be invoked.
        /// </summary>
        void RaiseCanExecute();
    }
}