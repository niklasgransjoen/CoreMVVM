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
        private readonly Dictionary<Type, MethodInfo> _methodCache = new Dictionary<Type, MethodInfo>();

        private readonly ILifetimeScope _lifetimeScope;

        public ViewLocator(ILifetimeScope lifetimeScope, DefaultViewProvider viewProvider)
        {
            _lifetimeScope = lifetimeScope;

            _viewProviders.Add(viewProvider);
        }

        #region Properties

        private string _initializeMethodName = "InitializeComponent";

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
                _methodCache.Clear();
            }
        }

        /// <summary>
        /// Gets or sets the name of the data context property to look for in constructed views.
        /// </summary>
        /// <value>The default is "DataContext".</value>
        public string DataContextPropertyName { get; set; } = "DataContext";

        #endregion Properties

        #region IViewLocator

        public object GetView<TViewModel>()
        {
            LoggerHelper.Debug($"View for view model '{typeof(TViewModel)} requested.");

            Type viewType = LocateViewType(provider => provider.FindView<TViewModel>());
            if (viewType is null)
            {
                LoggerHelper.Error($"Failed to find view for view model of type '{typeof(TViewModel)}'.");
                throw new InvalidOperationException($"No view found for view model of type '{typeof(TViewModel)}'.");
            }

            TViewModel viewModel = _lifetimeScope.Resolve<TViewModel>();
            return CreateView(viewType, viewModel);
        }

        public object GetView(object viewModel)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            LoggerHelper.Debug($"View for view model '{viewModel.GetType()} requested.");

            Type viewModelType = viewModel.GetType();

            Type viewType = LocateViewType(provider => provider.FindView(viewModelType));
            if (viewType is null)
            {
                LoggerHelper.Error($"Failed to find view for view model of type '{viewModel.GetType()}'.");
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
            LoggerHelper.Debug($"Resolved to instance of '{view.GetType()}'.");

            var dataContextProperty = viewType.GetProperty(DataContextPropertyName);

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
                dataContextProperty.SetValue(view, viewModel);
            }

            InitializeComponent(viewType, view);

            return view;
        }

        private void InitializeComponent(Type viewType, object instance)
        {
            if (!_methodCache.TryGetValue(viewType, out MethodInfo method))
            {
                method = instance.GetType()
                                 .GetMethod(InitializeMethodName, BindingFlags.Instance | BindingFlags.Public);

                _methodCache[viewType] = method;
            }

            method?.Invoke(instance, null);
        }

        #endregion Helpers
    }
}