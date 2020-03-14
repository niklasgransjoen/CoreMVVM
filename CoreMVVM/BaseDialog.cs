using CoreMVVM.Threading;
using CoreMVVM.Validation;
using System;
using System.Threading.Tasks;

namespace CoreMVVM
{
    /// <summary>
    /// Base for dialog view models.
    /// </summary>
    public abstract class BaseDialog : BaseValidationModel, IDialog
    {
        private readonly TaskCompletionSource<int> _taskCompletionSource = new TaskCompletionSource<int>();

        protected BaseDialog()
        {
        }

        #region Events

        /// <summary>
        /// Occurs when the dialog is closed.
        /// </summary>
        public event Action Closed;

        #endregion Events

        #region Properties

        /// <summary>
        /// A task promising the result of the dialog.
        /// </summary>
        public RebelTask Task => _taskCompletionSource.Task;

        #endregion Properties

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        protected void Close()
        {
            if (_taskCompletionSource.Task.IsCompleted)
                return;

            _taskCompletionSource.SetResult(0);
            Closed?.Invoke();
        }
    }

    /// <summary>
    /// Base for dialog view models.
    /// </summary>
    public abstract class BaseDialog<TResult> : BaseValidationModel, IDialog<TResult>
    {
        private readonly TaskCompletionSource<TResult> _taskCompletionSource = new TaskCompletionSource<TResult>();

        protected BaseDialog()
        {
        }

        #region Events

        /// <summary>
        /// Occurs when the dialog is closed.
        /// </summary>
        public event Action Closed;

        #endregion Events

        #region Properties

        /// <summary>
        /// A task promising the result of the dialog.
        /// </summary>
        public RebelTask<TResult> Task => _taskCompletionSource.Task;

        /// <summary>
        /// A task promising the result of the dialog.
        /// </summary>
        RebelTask IDialog.Task => Task;

        #endregion Properties

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        /// <param name="result"></param>
        protected void Close(TResult result)
        {
            if (_taskCompletionSource.Task.IsCompleted)
                return;

            _taskCompletionSource.SetResult(result);
            Closed?.Invoke();
        }
    }
}