using System;
using System.Windows;

namespace CoreMVVM.Windows.FallbackImplementations
{
    /// <summary>
    /// Default implementation of <see cref="IWindowManager"/>.
    /// </summary>
    public class WindowManager : IWindowManager
    {
        private readonly IViewLocator _viewLocator;

        public WindowManager(IViewLocator viewLocator)
        {
            _viewLocator = viewLocator;
        }

        #region IWindowManager

        public virtual Window Show(Type viewModelType, WindowType windowType, Window? owner = null)
        {
            if (viewModelType is null)
                throw new ArgumentNullException(nameof(viewModelType));

            Window window = GetWindow(viewModelType, owner);

            if (windowType == WindowType.Dialog)
                window.ShowDialog();
            else
                window.Show();

            return window;
        }

        public virtual Window Show(object viewModel, WindowType windowType, Window? owner = null)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            Window window = GetWindow(viewModel, owner);

            if (windowType == WindowType.Dialog)
                window.ShowDialog();
            else
                window.Show();

            return window;
        }

        #endregion IWindowManager

        /// <summary>
        /// Resolves the window for the given view model type.
        /// </summary>
        protected virtual Window GetWindow(Type viewModelType, Window? owner)
        {
            var window = _viewLocator.ResolveView(viewModelType);
            return CastWindowAndAssignOwner(viewModelType, window, owner);
        }

        /// <summary>
        /// Resolves the window for the given view model.
        /// </summary>
        protected virtual Window GetWindow(object viewModel, Window? owner)
        {
            if (viewModel is null) throw new ArgumentNullException(nameof(viewModel));
            if (owner is null) throw new ArgumentNullException(nameof(owner));

            var window = _viewLocator.ResolveView(viewModel);
            return CastWindowAndAssignOwner(viewModel.GetType(), window, owner);
        }

        private static Window CastWindowAndAssignOwner(Type viewModelType, object view, Window? owner)
        {
            if (!(view is Window window))
                throw new InvalidOperationException($"View resolved for view model '{viewModelType}' is of type '{view.GetType()}', which does not inherit from '{typeof(Window)}'.");

            window.Owner = owner;

            return window;
        }
    }
}