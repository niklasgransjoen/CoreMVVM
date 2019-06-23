using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Implements logic for managing windows.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="owner">The owner of the returned window.</param>
        /// <param name="scope">The optional scope of the window.</param>
        Window ShowWindow<TViewModel>(Window owner = null);

        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        /// <param name="scope">The optional scope of the window.</param>
        Window ShowWindow(object viewModel, Window owner = null);
    }
}