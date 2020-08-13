using CoreMVVM.IOC;
using System;
using System.Windows;

namespace CoreMVVM.Windows.FallbackImplementations
{
    public class WindowManager : IWindowManager
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IViewLocator _viewLocator;

        public WindowManager(ILifetimeScope lifetimeScope, IViewLocator viewLocator)
        {
            _lifetimeScope = lifetimeScope;
            _viewLocator = viewLocator;
        }

        #region IWindowManager

        public virtual Window ShowWindow(Type viewModelType, Window? owner = null)
        {
            if (viewModelType is null)
                throw new ArgumentNullException(nameof(viewModelType));

            Window window = GetWindow(viewModelType, owner);
            window.Show();

            return window;
        }

        public virtual Window ShowWindow(object viewModel, Window? owner = null)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            Window window = GetWindow(viewModel, owner);
            window.Show();

            return window;
        }

        public virtual Window ShowDialog(Type viewModelType, Window? owner = null)
        {
            if (viewModelType is null)
                throw new ArgumentNullException(nameof(viewModelType));

            Window window = GetWindow(viewModelType, owner);
            window.ShowDialog();

            return window;
        }

        public virtual Window ShowDialog(object viewModel, Window? owner = null)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            Window window = GetWindow(viewModel, owner);
            window.ShowDialog();

            return window;
        }

        #endregion IWindowManager

        protected virtual Window GetWindow(Type viewModelType, Window? owner)
        {
            var window = _viewLocator.ResolveView(viewModelType);
            return InitializeWindow(viewModelType, window, owner);
        }

        protected virtual Window GetWindow(object viewModel, Window? owner)
        {
            var window = _viewLocator.ResolveView(viewModel);
            return InitializeWindow(viewModel.GetType(), window, owner);
        }

        protected virtual Window InitializeWindow(Type viewModelType, object view, Window? owner)
        {
            if (!(view is Window window))
                throw new InvalidOperationException($"View resolved for view model '{viewModelType}' is of type '{view.GetType()}', which does not inherit from '{typeof(Window)}'.");

            ControlServiceProvider.SetServiceProvider(window, _lifetimeScope);
            window.Owner = owner;

            return window;
        }
    }
}