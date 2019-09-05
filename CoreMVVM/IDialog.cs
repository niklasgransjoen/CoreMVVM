using System;
using System.Threading.Tasks;

namespace CoreMVVM
{
    /// <summary>
    /// Implements logic for a simlpe dialog.
    /// </summary>
    public interface IDialog
    {
        /// <summary>
        /// Occurs when the dialog is closed.
        /// </summary>
        event Action Closed;

        /// <summary>
        /// A task promising the result of the dialog.
        /// </summary>
        Task Task { get; }
    }

    /// <summary>
    /// Implements logic for a dialog with a result.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IDialog<TResult> : IDialog
    {
        /// <summary>
        /// A task promising the result of the dialog.
        /// </summary>
        new Task<TResult> Task { get; }
    }
}