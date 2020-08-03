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
            if (windowManager is null)
                throw new ArgumentNullException(nameof(windowManager));

            return windowManager.ShowWindow(typeof(TViewModel), owner);
        }

        /// <summary>
        /// Shows a window for the given view model type as a dialog.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="owner">The owner of the returned window.</param>
        public static Window ShowDialog<TViewModel>(this IWindowManager windowManager, Window? owner = null) where TViewModel : class
        {
            if (windowManager is null)
                throw new ArgumentNullException(nameof(windowManager));

            return windowManager.ShowDialog(typeof(TViewModel), owner);
        }
    }
}