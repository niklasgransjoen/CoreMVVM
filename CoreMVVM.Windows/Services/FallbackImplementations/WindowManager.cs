using System;
using System.Windows;

namespace CoreMVVM.Windows.FallbackImplementations
{
    public class WindowManager : IWindowManager
    {
        private readonly IViewLocator _viewLocator;

        public WindowManager(IViewLocator viewLocator)
        {
            _viewLocator = viewLocator;
        }

        #region IWindowManager

        public Window ShowWindow(Type viewModelType, Window owner = null)
        {
            if (viewModelType is null)
                throw new ArgumentNullException(nameof(viewModelType));

            Window window = GetWindow(viewModelType, owner);
            window.Show();

            return window;
        }

        public Window ShowWindow(object viewModel, Window owner = null)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            Window window = GetWindow(viewModel, owner);
            window.Show();

            return window;
        }

        public Window ShowDialog(Type viewModelType, Window owner = null)
        {
            if (viewModelType is null)
                throw new ArgumentNullException(nameof(viewModelType));

            Window window = GetWindow(viewModelType, owner);
            window.ShowDialog();

            return window;
        }

        public Window ShowDialog(object viewModel, Window owner = null)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            Window window = GetWindow(viewModel, owner);
            window.ShowDialog();

            return window;
        }

        #endregion IWindowManager

        private Window GetWindow(Type viewModelType, Window owner)
        {
            var window = _viewLocator.ResolveView(viewModelType);
            return CastAndAssignOwner(viewModelType, window, owner);
        }

        private Window GetWindow(object viewModel, Window owner)
        {
            var window = _viewLocator.ResolveView(viewModel);
            return CastAndAssignOwner(viewModel.GetType(), window, owner);
        }

        private Window CastAndAssignOwner(Type viewModelType, object view, Window owner)
        {
            if (!(view is Window window))
                throw new InvalidOperationException($"View resolved for view model '{viewModelType}' is of type '{view.GetType()}', which does not inherit from '{typeof(Window)}'.");

            window.Owner = owner;
            return window;
        }
    }
}