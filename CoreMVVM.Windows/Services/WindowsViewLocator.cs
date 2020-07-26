using CoreMVVM.Implementations;
using CoreMVVM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// This implementation of the <see cref="IViewLocator"/> attempts to assign the view model to the resolved view,
    /// if the view is of type <see cref="FrameworkElement"/>.
    /// </summary>
    [Scope(ComponentScope.Singleton)]
    public class WindowsViewLocator : IViewLocator
    {
        private readonly List<IViewProvider> _viewProviders = new List<IViewProvider>();
        private readonly Dictionary<Type, Type> _viewCache = new Dictionary<Type, Type>();
        private readonly List<Action<object, object>> _onResolveActions = new List<Action<object, object>>();

        private readonly IContainer _container;

        #region Constructors

        public WindowsViewLocator(IContainer container, DefaultViewProvider viewProvider)
        {
            _container = container;

            _viewProviders.Add(viewProvider);
        }

        #endregion Constructors

        #region IViewLocator

        #region ResolveView

        public object ResolveView(Type viewModelType)
        {
            if (viewModelType is null)
                throw new ArgumentNullException(nameof(viewModelType));

            Type viewType = ResolveViewType(viewModelType);

            var viewModel = _container.ResolveRequiredService(viewModelType);
            return CreateView(viewType, viewModel);
        }

        public object ResolveView(object viewModel)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            Type viewModelType = viewModel.GetType();
            Type viewType = ResolveViewType(viewModelType);

            return CreateView(viewType, viewModel);
        }

        public Type ResolveViewType(Type viewModelType)
        {
            if (_viewCache.TryGetValue(viewModelType, out var viewType))
                return viewType;

            var result = LocateViewType((provider, context) => provider.FindView(viewModelType, context));
            if (result is null)
            {
                throw new ViewNotFoundException($"No view found for view model of type '{viewModelType}'.");
            }

            if (result.CacheView)
                _viewCache[viewModelType] = result.ViewType;

            return result.ViewType;
        }

        #endregion ResolveView

        #region TryResolveView

        public bool TryResolveView(Type viewModelType, out object view)
        {
            if (!TryResolveViewType(viewModelType, out Type viewType))
            {
                view = null;
                return false;
            }

            var viewModel = _container.ResolveService(viewModelType);
            if (viewModel is null)
            {
                view = null;
                return false;
            }

            view = TryCreateView(viewType, viewModel);
            return !(view is null);
        }

        public bool TryResolveView(object viewModel, out object view)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            Type viewModelType = viewModel.GetType();
            if (!TryResolveViewType(viewModelType, out Type viewType))
            {
                view = null;
                return false;
            }

            view = TryCreateView(viewType, viewModel);
            return !(view is null);
        }

        public bool TryResolveViewType(Type viewModelType, out Type viewType)
        {
            if (_viewCache.TryGetValue(viewModelType, out viewType))
                return true;

            var result = LocateViewType((provider, context) => provider.FindView(viewModelType, context));
            if (result is null)
            {
                viewType = null;
                return false;
            }

            if (result.CacheView)
                _viewCache[viewModelType] = result.ViewType;

            viewType = result.ViewType;
            return true;
        }

        #endregion TryResolveView

        public void AddViewProvider(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!typeof(IViewProvider).IsAssignableFrom(type))
                throw new ArgumentException($"Type '{type}' does not implement required interface '{typeof(IViewProvider)}'.", nameof(type));

            var viewProvider = (IViewProvider)_container.ResolveRequiredService(type);
            AddViewProvider(viewProvider);
        }

        public void AddViewProvider(IViewProvider viewProvider)
        {
            if (viewProvider is null)
                throw new ArgumentNullException(nameof(viewProvider));

            _viewProviders.Add(viewProvider);
        }

        #endregion IViewLocator

        #region Methods

        /// <summary>
        /// Adds an action that gets performed on the resolved view before it's returned.
        /// </summary>
        /// <param name="action">The action to perform. The first argument is the view model, the second is the view.</param>
        /// <remarks>
        /// Actions are executed in the order they are added. They are executed after DataContext is assigned (if view is FrameworkElement).
        /// </remarks>
        public void AddOnResolve(Action<object, object> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            _onResolveActions.Add(action);
        }

        #endregion Methods

        #region Helpers

        private ViewProviderContext LocateViewType(Func<IViewProvider, ViewProviderContext, bool> locator)
        {
            ViewProviderContext context = new ViewProviderContext();
            foreach (var viewProvider in Enumerable.Reverse(_viewProviders))
            {
                // Keep going until a provider finds the view.
                if (!locator(viewProvider, context))
                    continue;

                if (context.ViewType is null)
                    throw new InvalidOperationException($"View provider '{viewProvider.GetType()}' returned true, but no view was provided.");

                return context;
            }

            return null;
        }

        private object CreateView(Type viewType, object viewModel)
        {
            object view = _container.ResolveRequiredService(viewType);

            TrySetDataContext(viewModel, view);
            _onResolveActions.ForEach(a => a(viewModel, view));

            return view;
        }

        private object TryCreateView(Type viewType, object viewModel)
        {
            object view = _container.ResolveService(viewType);

            TrySetDataContext(viewModel, view);
            _onResolveActions.ForEach(a => a(viewModel, view));

            return view;
        }

        private static void TrySetDataContext(object viewModel, object view)
        {
            if (view is FrameworkElement frameworkElement)
            {
                frameworkElement.DataContext = viewModel;
            }
        }

        #endregion Helpers
    }
}