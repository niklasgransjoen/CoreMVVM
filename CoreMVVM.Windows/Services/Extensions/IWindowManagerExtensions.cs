using System;
using System.Windows;

namespace CoreMVVM.Windows
{
    public static class IWindowManagerExtensions
    {
        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="owner">The owner of the returned window.</param>
        public static Window ShowWindow<TViewModel>(this IWindowManager windowManager, Window? owner = null) where TViewModel : class
        {
            return ShowWindow(windowManager, typeof(TViewModel), owner);
        }

        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <param name="viewModelType">The type of the view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        public static Window ShowWindow(this IWindowManager windowManager, Type viewModelType, Window? owner = null)
        {
            if (windowManager is null)
                throw new ArgumentNullException(nameof(windowManager));

            return windowManager.Show(viewModelType, WindowType.Window, owner);
        }

        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        public static Window ShowWindow(this IWindowManager windowManager, object viewModel, Window? owner = null)
        {
            if (windowManager is null)
                throw new ArgumentNullException(nameof(windowManager));

            return windowManager.Show(viewModel, WindowType.Window, owner);
        }

        /// <summary>
        /// Shows a window for the given view model type as a dialog.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="owner">The owner of the returned window.</param>
        public static Window ShowDialog<TViewModel>(this IWindowManager windowManager, Window? owner = null) where TViewModel : class
        {
            return ShowDialog(windowManager, typeof(TViewModel), owner);
        }

        /// <summary>
        /// Shows a window for the given view model type as a dialog.
        /// </summary>
        /// <param name="viewModelType">The type of the view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        public static Window ShowDialog(this IWindowManager windowManager, Type viewModelType, Window? owner = null)
        {
            if (windowManager is null)
                throw new ArgumentNullException(nameof(windowManager));

            return windowManager.Show(viewModelType, WindowType.Dialog, owner);
        }

        /// <summary>
        /// Shows a window for the given view model type as a dialog.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        public static Window ShowDialog(this IWindowManager windowManager, object viewModel, Window? owner = null)
        {
            if (windowManager is null)
                throw new ArgumentNullException(nameof(windowManager));

            return windowManager.Show(viewModel, WindowType.Dialog, owner);
        }
    }
}