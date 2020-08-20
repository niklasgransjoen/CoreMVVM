using CoreMVVM.IOC;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CoreMVVM.Implementations
{
    /// <summary>
    /// Default implementation of the <see cref="IViewLocator"/> service.
    /// </summary>
    public sealed class ViewLocator : IViewLocator
    {
        private readonly Dictionary<Type, Type> _viewCache = new Dictionary<Type, Type>();

        private readonly ILifetimeScope _lifetimeScope;
        private readonly IViewProvider[] _viewProviders;
        private readonly IEnumerable<IViewInitializer> _viewInitializers;

        public ViewLocator(ILifetimeScope lifetimeScope, IEnumerable<IViewProvider> viewProviders, IEnumerable<IViewInitializer> viewInitializers)
        {
            _lifetimeScope = lifetimeScope;

            // Reverse, so resolve is done in reverse order of registrations.
            _viewProviders = viewProviders.Reverse().ToArray();

            _viewInitializers = viewInitializers;
        }

        #region IViewLocator

        #region ResolveView

        public object ResolveView(Type viewModelType)
        {
            if (viewModelType is null)
                throw new ArgumentNullException(nameof(viewModelType));

            Type viewType = ResolveViewType(viewModelType);

            var viewModel = _lifetimeScope.ResolveRequiredService(viewModelType);
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

            var viewModel = _lifetimeScope.ResolveService(viewModelType);
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

        #endregion IViewLocator

        #region Helpers

        private ViewProviderContext? LocateViewType(Type viewModelType)
        {
            ViewProviderContext context = new ViewProviderContext(viewModelType);
            foreach (var viewProvider in _viewProviders)
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
            object view = _lifetimeScope.ResolveRequiredService(viewType);

            foreach (var initializer in _viewInitializers)
            {
                initializer.InitView(viewModel, view);
            }

            return view;
        }

        private object? TryCreateView(Type viewType, object viewModel)
        {
            object? view = _lifetimeScope.ResolveService(viewType);
            if (view is null)
                return null;

            foreach (var initializer in _viewInitializers)
            {
                initializer.InitView(viewModel, view);
            }

            return view;
        }

        #endregion Helpers
    }
}