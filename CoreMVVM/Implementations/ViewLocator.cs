using CoreMVVM.IOC;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CoreMVVM.Implementations
{
    /// <summary>
    /// The default implementation of the <see cref="IViewLocator"/> service.
    /// </summary>
    [Scope(ComponentScope.Singleton)]
    public sealed class ViewLocator : IViewLocator
    {
        private readonly List<IViewProvider> _viewProviders = new List<IViewProvider>();

        private readonly Dictionary<Type, Type> _viewCache = new Dictionary<Type, Type>();
        private readonly List<Action<object, object>> _onResolveActions = new List<Action<object, object>>();

        private readonly IContainer _container;

        public ViewLocator(IContainer container, DefaultViewProvider viewProvider)
        {
            _container = container;

            _viewProviders.Add(viewProvider);
        }

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

            var result = LocateViewType(viewModelType);
            if (result is null)
            {
                throw new ViewNotFoundException($"No view found for view model of type '{viewModelType}'.");
            }

            if (result.CacheView)
                _viewCache[viewModelType] = result.ViewType!;

            return result.ViewType!;
        }

        #endregion ResolveView

        #region TryResolveView

        public bool TryResolveView(Type viewModelType, [NotNullWhen(true)] out object? view)
        {
            if (!TryResolveViewType(viewModelType, out var viewType))
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

        public bool TryResolveView(object viewModel, [NotNullWhen(true)] out object? view)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            Type viewModelType = viewModel.GetType();
            if (!TryResolveViewType(viewModelType, out var viewType))
            {
                view = null;
                return false;
            }

            view = TryCreateView(viewType, viewModel);
            return !(view is null);
        }

        public bool TryResolveViewType(Type viewModelType, [NotNullWhen(true)] out Type? viewType)
        {
            if (_viewCache.TryGetValue(viewModelType, out viewType))
                return true;

            var result = LocateViewType(viewModelType);
            if (result is null)
            {
                viewType = null;
                return false;
            }

            if (result.CacheView)
                _viewCache[viewModelType] = result.ViewType!;

            viewType = result.ViewType!;
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
        /// Actions are executed in the order they are added.
        /// </remarks>
        public void AddOnResolve(Action<object, object> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            _onResolveActions.Add(action);
        }

        #endregion Methods

        #region Helpers

        private ViewProviderContext? LocateViewType(Type viewModelType)
        {
            ViewProviderContext context = new ViewProviderContext(viewModelType);
            foreach (var viewProvider in Enumerable.Reverse(_viewProviders))
            {
                // Keep going until a provider finds the view.
                viewProvider.FindView(context);
                if (context.ViewType is null)
                    continue;

                return context;
            }

            return null;
        }

        private object CreateView(Type viewType, object viewModel)
        {
            object view = _container.ResolveRequiredService(viewType);

            _onResolveActions.ForEach(a => a(viewModel, view));

            return view;
        }

        private object TryCreateView(Type viewType, object viewModel)
        {
            object view = _container.ResolveService(viewType);

            _onResolveActions.ForEach(a => a(viewModel, view));

            return view;
        }

        #endregion Helpers
    }
}