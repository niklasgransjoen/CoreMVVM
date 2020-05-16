using CoreMVVM.IOC;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Implements logic for managing windows.
    /// </summary>
    [FallbackImplementation(typeof(FallbackImplementations.WindowManager))]
    public interface IWindowManager
    {
        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="owner">The owner of the returned window.</param>
        Window ShowWindow<TViewModel>(Window owner = null) where TViewModel : class;

        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        Window ShowWindow(object viewModel, Window owner = null);

        /// <summary>
        /// Shows a window for the given view model type as a dialog.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="owner">The owner of the returned window.</param>
        Window ShowDialog<TViewModel>(Window owner = null) where TViewModel : class;

        /// <summary>
        /// Shows a window for the given view model type as a dialog.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        Window ShowDialog(object viewModel, Window owner = null);
    }
}