using CoreMVVM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreMVVM.Implementations
{
    [Scope(ComponentScope.Singleton)]
    public sealed class ViewLocator : IViewLocator
    {
        private readonly List<IViewProvider> _viewProviders = new List<IViewProvider>();

        private readonly Dictionary<Type, Type> _viewCache = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, PropertyInfo> _dataContextCache = new Dictionary<Type, PropertyInfo>();
        private readonly Dictionary<Type, MethodInfo> _initMethodCache = new Dictionary<Type, MethodInfo>();

        private readonly ILifetimeScope _lifetimeScope;

        public ViewLocator(ILifetimeScope lifetimeScope, DefaultViewProvider viewProvider)
        {
            _lifetimeScope = lifetimeScope;

            _viewProviders.Add(viewProvider);
        }

        #region Properties

        private string _initializeMethodName = "InitializeComponent";
        private string _dataContextPropertyName = "DataContext";

        /// <summary>
        /// Gets or sets the name of the initialize method to look for in constructed views.
        /// </summary>
        /// <value>The default is "InitializeComponent".</value>
        public string InitializeMethodName
        {
            get => _initializeMethodName;
            set
            {
                _initializeMethodName = value;
                _initMethodCache.Clear();
            }
        }

        /// <summary>
        /// Gets or sets the name of the data context property to look for in constructed views.
        /// </summary>
        /// <value>The default is "DataContext".</value>
        public string DataContextPropertyName
        {
            get => _dataContextPropertyName;
            set
            {
                _dataContextPropertyName = value;
                _dataContextCache.Clear();
            }
        }

        #endregion Properties

        #region IViewLocator

        public object GetView<TViewModel>() where TViewModel : class
        {
            LoggerHelper.Debug($"View for view model '{typeof(TViewModel)} requested.");

            Type viewType = GetViewType<TViewModel>();

            TViewModel viewModel = _lifetimeScope.Resolve<TViewModel>();
            return CreateView(viewType, viewModel);
        }

        public object GetView(object viewModel)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            LoggerHelper.Debug($"View for view model '{viewModel.GetType()} requested.");

            Type viewModelType = viewModel.GetType();
            Type viewType = GetViewType(viewModelType);

            return CreateView(viewType, viewModel);
        }

        public void AddViewProvider<TViewProvider>() where TViewProvider : class, IViewProvider
        {
            var viewProvider = _lifetimeScope.Resolve<TViewProvider>();
            _viewProviders.Add(viewProvider);
        }

        public void AddViewProvider(IViewProvider viewProvider)
        {
            if (viewProvider is null)
                throw new ArgumentNullException(nameof(viewProvider));

            _viewProviders.Add(viewProvider);
        }

        #endregion IViewLocator

        #region Helpers

        private Type GetViewType<TViewModel>() where TViewModel : class
        {
            if (_viewCache.TryGetValue(typeof(TViewModel), out var viewType))
                return viewType;

            var result = LocateViewType((provider, context) => provider.FindView<TViewModel>(context));
            if (result is null)
            {
                LoggerHelper.Error($"Failed to find view for view model of type '{typeof(TViewModel)}'.");
                throw new InvalidOperationException($"No view found for view model of type '{typeof(TViewModel)}'.");
            }

            if (result.CacheView)
                _viewCache[typeof(TViewModel)] = result.ViewType;

            return result.ViewType;
        }

        private Type GetViewType(Type viewModelType)
        {
            if (_viewCache.TryGetValue(viewModelType, out var viewType))
                return viewType;

            var result = LocateViewType((provider, context) => provider.FindView(viewModelType, context));
            if (result is null)
            {
                LoggerHelper.Error($"Failed to find view for view model of type '{viewModelType}'.");
                throw new InvalidOperationException($"No view found for view model of type '{viewModelType}'.");
            }

            if (result.CacheView)
                _viewCache[viewModelType] = result.ViewType;

            return result.ViewType;
        }

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
            object view = _lifetimeScope.Resolve(viewType);
            LoggerHelper.Debug($"Resolved to instance of '{view.GetType()}'.");

            TrySetDataContext(view, viewModel);
            InitializeComponent(viewType, view);

            return view;
        }

        private bool TrySetDataContext(object view, object viewModel)
        {
            Type viewType = view.GetType();
            if (!_dataContextCache.TryGetValue(viewType, out PropertyInfo dataContextProperty))
            {
                dataContextProperty = viewType.GetProperty(DataContextPropertyName);
                if (dataContextProperty is null)
                {
                    LoggerHelper.Log($"View does not have DataContext property with name '{DataContextPropertyName}'.");
                }
                else if (dataContextProperty.PropertyType != typeof(object))
                {
                    LoggerHelper.Log($"Property '{DataContextPropertyName}' is not of type '{typeof(object)}'.");
                }
                else
                {
                    _dataContextCache[viewType] = dataContextProperty;
                }
            }
            if (dataContextProperty is null)
                return false;

            dataContextProperty.SetValue(view, viewModel);
            return true;
        }

        private void InitializeComponent(Type viewType, object instance)
        {
            if (!_initMethodCache.TryGetValue(viewType, out MethodInfo method))
            {
                method = instance.GetType()
                                 .GetMethod(InitializeMethodName, BindingFlags.Instance | BindingFlags.Public);

                _initMethodCache[viewType] = method;
            }

            method?.Invoke(instance, null);
        }

        #endregion Helpers
    }
}