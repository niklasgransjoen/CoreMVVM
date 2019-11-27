using CoreMVVM.Implementations;
using CoreMVVM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Provides methods for retrieving a view instance from a given view model.
    /// </summary>
    public class WindowsViewLocator : IViewLocator
    {
        private readonly List<IViewProvider> _viewProviders = new List<IViewProvider>();
        private readonly Dictionary<Type, MethodInfo> _methodCache = new Dictionary<Type, MethodInfo>();

        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger _logger;

        #region Constructors

        public WindowsViewLocator(ILifetimeScope lifetimeScope, ILogger logger, DefaultViewProvider viewProvider)
        {
            _lifetimeScope = lifetimeScope;
            _logger = logger;

            _viewProviders.Add(viewProvider);
        }

        #endregion Constructors

        #region IViewLocator

        public object GetView<TViewModel>()
        {
            _logger.Debug($"View for view model '{typeof(TViewModel)} requested.");

            Type viewType = LocateViewType(provider => provider.FindView<TViewModel>());
            if (viewType is null)
            {
                _logger.Error($"Failed to find view for view model of type '{typeof(TViewModel)}'.");
                throw new InvalidOperationException($"No view found for view model of type '{typeof(TViewModel)}'.");
            }

            TViewModel viewModel = _lifetimeScope.Resolve<TViewModel>();
            return CreateView(viewType, viewModel);
        }

        public object GetView(object viewModel)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            _logger.Debug($"View for view model '{viewModel.GetType()} requested.");

            Type viewModelType = viewModel.GetType();

            Type viewType = LocateViewType(provider => provider.FindView(viewModelType));
            if (viewType is null)
            {
                _logger.Error($"Failed to find view for view model of type '{viewModel.GetType()}'.");
                throw new InvalidOperationException($"No view found for view model of type '{viewModel.GetType()}'.");
            }

            return CreateView(viewType, viewModel);
        }

        public void AddViewProvider<TViewProvider>() where TViewProvider : IViewProvider
        {
            var viewProvider = _lifetimeScope.Resolve<TViewProvider>();
            AddViewProvider(viewProvider);
        }

        public void AddViewProvider(IViewProvider viewProvider)
        {
            _viewProviders.Add(viewProvider);
        }

        #endregion IViewLocator

        #region Helpers

        private Type LocateViewType(Func<IViewProvider, Type> locator)
        {
            foreach (var viewProvider in Enumerable.Reverse(_viewProviders))
            {
                Type viewType = locator(viewProvider);
                if (viewType != null)
                    return viewType;
            }

            return null;
        }

        private object CreateView(Type viewType, object viewModel)
        {
            object view = _lifetimeScope.Resolve(viewType);
            _logger.Debug($"Resolved to instance of '{view.GetType()}'.");

            if (view is DependencyObject depObj)
            {
                LifetimeScopePropertyExtention.SetLifetimeScope(depObj, _lifetimeScope);

                if (view is FrameworkElement frameworkElement)
                    frameworkElement.DataContext = viewModel;
            }

            InitializeComponent(viewType, view);

            return view;
        }

        private void InitializeComponent(Type viewType, object instance)
        {
            if (!_methodCache.TryGetValue(viewType, out MethodInfo method))
            {
                method = instance.GetType()
                                 .GetMethod("InitializeComponent", BindingFlags.Instance | BindingFlags.Public);

                _methodCache[viewType] = method;
            }

            method?.Invoke(instance, null);
        }

        #endregion Helpers
    }
}