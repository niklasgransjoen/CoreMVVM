using CoreMVVM.IOC;
using System;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Specifies the type of a window.
    /// </summary>
    public enum WindowType
    {
        /// <summary>
        /// The window is a normal, standalone window.
        /// </summary>
        Window,

        /// <summary>
        /// The window is a dialog.
        /// </summary>
        Dialog
    }

    /// <summary>
    /// Implements logic for managing windows.
    /// </summary>
    [FallbackImplementation(typeof(FallbackImplementations.WindowManager))]
    public interface IWindowManager
    {
        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <param name="viewModelType">The type of the view model.</param>
        /// <param name="windowType">Specifies the type of window to show.</param>
        /// <param name="owner">The owner of the returned window.</param>
        Window Show(Type viewModelType, WindowType windowType, Window? owner = null);

        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="windowType">Specifies the type of window to show.</param>
        /// <param name="owner">The owner of the returned window.</param>
        Window Show(object viewModel, WindowType windowType, Window? owner = null);
    }
}