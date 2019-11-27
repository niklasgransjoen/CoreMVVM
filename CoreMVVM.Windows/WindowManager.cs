using CoreMVVM.IOC;
using System.Windows;

namespace CoreMVVM.Windows
{
    [Scope(ComponentScope.Singleton)]
    public class WindowManager : IWindowManager
    {
        private readonly IViewLocator _viewLocator;

        public WindowManager(IViewLocator viewLocator)
        {
            _viewLocator = viewLocator;
        }

        #region IWindowManager

        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="owner">The owner of the returned window.</param>
        public Window ShowWindow<TViewModel>(Window owner = null) where TViewModel : class
        {
            Window window = GetWindow<TViewModel>(owner);
            window.Show();

            return window;
        }

        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        public Window ShowWindow(object viewModel, Window owner = null)
        {
            Window window = GetWindow(viewModel, owner);
            window.Show();

            return window;
        }

        /// <summary>
        /// Shows a window for the given view model type as a dialog.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="owner">The owner of the returned window.</param>
        public Window ShowDialog<TViewModel>(Window owner = null) where TViewModel : class
        {
            Window window = GetWindow<TViewModel>(owner);
            window.ShowDialog();

            return window;
        }

        /// <summary>
        /// Shows a window for the given view model type as a dialog.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        public Window ShowDialog(object viewModel, Window owner = null)
        {
            Window window = GetWindow(viewModel, owner);
            window.ShowDialog();

            return window;
        }

        #endregion IWindowManager

        private Window GetWindow<TViewModel>(Window owner) where TViewModel : class
        {
            Window window = (Window)_viewLocator.GetView<TViewModel>();
            window.Owner = owner;

            return window;
        }

        private Window GetWindow(object viewModel, Window owner)
        {
            var window = (Window)_viewLocator.GetView(viewModel);
            window.Owner = owner;

            return window;
        }
    }
}