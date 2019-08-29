using System.Windows;

namespace CoreMVVM.Windows
{
    public class WindowManager : IWindowManager
    {
        private readonly IViewLocator _viewLocator;

        public WindowManager(IViewLocator viewLocator)
        {
            _viewLocator = viewLocator;
        }

        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="owner">The owner of the returned window.</param>
        /// <param name="scope">The optional scope of the window.</param>
        public Window ShowWindow<TViewModel>(Window owner = null)
        {
            Window window = (Window)_viewLocator.GetView<TViewModel>();
            window.Owner = owner;
            window.Show();
            return window;
        }

        /// <summary>
        /// Shows a window for the given view model type.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="owner">The owner of the returned window.</param>
        /// <param name="scope">The optional scope of the window.</param>
        public Window ShowWindow(object viewModel, Window owner = null)
        {
            var window = (Window)_viewLocator.GetView(viewModel);
            window.Owner = owner;
            window.Show();
            return window;
        }
    }
}