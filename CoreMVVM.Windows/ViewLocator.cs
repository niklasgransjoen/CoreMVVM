using CoreMVVM.IOC;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Provides methods for retrieving a view instance from a given view model.
    /// </summary>
    public class ViewLocator : IViewLocator
    {
        private readonly IContainer _container;
        private readonly ILogger _logger;

        private readonly Dictionary<Type, Type> _registeredViews = new Dictionary<Type, Type>();

        #region Constructors

        public ViewLocator(IContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;

            GetViewTypeNameFromViewModelTypeName = (viewModelTypeName) =>
            {
                return viewModelTypeName
                    .Replace("ViewModel", "View")
                    .Replace("WindowModel", "Window");
            };

            GetViewTypeFromViewModelType = viewModelType =>
            {
                string viewTypeName = GetViewTypeNameFromViewModelTypeName(viewModelType.FullName);
                return viewModelType.Assembly.GetType(viewTypeName);
            };
        }

        #endregion Constructors

        #region Properties

        private Func<Type, Type> _getViewTypeFromViewModelType;
        private Func<string, string> _getViewTypeNameFromViewModelTypeName;

        /// <summary>
        /// Returns the type of a view based on the type of the view model.
        /// </summary>
        /// <exception cref="ArgumentNullException">If attempted to set to null.</exception>
        public Func<Type, Type> GetViewTypeFromViewModelType
        {
            get => _getViewTypeFromViewModelType;
            set => _getViewTypeFromViewModelType = value ?? throw new ArgumentNullException(nameof(GetViewTypeFromViewModelType));
        }

        /// <summary>
        /// Returns the name of a view based on the name of a view model.
        /// </summary>
        /// <exception cref="ArgumentNullException">If attempted to set to null.</exception>
        public Func<string, string> GetViewTypeNameFromViewModelTypeName
        {
            get => _getViewTypeNameFromViewModelTypeName;
            set => _getViewTypeNameFromViewModelTypeName = value ?? throw new ArgumentNullException(nameof(GetViewTypeNameFromViewModelTypeName));
        }

        #endregion Properties

        #region Methods

        #region GetView

        /// <summary>
        /// Gets the view for the view model of a given type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        public object GetView<TViewModel>()
        {
            TViewModel viewModel = _container.Resolve<TViewModel>();
            return GetView(viewModel);
        }

        /// <summary>
        /// Gets the view for the given view model.
        /// </summary>
        public object GetView(object viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            _logger.Debug($"View for view model '{viewModel.GetType()} requested.");
            bool isRegistered = _registeredViews.TryGetValue(viewModel.GetType(), out Type registeredViewType);
            if (isRegistered)
                return CreateView(registeredViewType, viewModel);

            Type viewType = GetViewTypeFromViewModelType(viewModel.GetType());
            if (viewType == null)
            {
                _logger.Error($"Failed to find view for view model of type '{viewModel.GetType()}'.");
                throw new InvalidOperationException($"No view found for view model of type '{viewModel.GetType()}'.");
            }

            return CreateView(viewType, viewModel);
        }

        #endregion GetView

        /// <summary>
        /// Registers a view to a view model.
        /// </summary>
        /// <typeparam name="TViewModel">The view model to register.</typeparam>
        /// <typeparam name="TView">The view to register.</typeparam>
        public void RegisterView<TViewModel, TView>()
        {
            _registeredViews[typeof(TViewModel)] = typeof(TView);
        }

        #endregion Methods

        private object CreateView(Type viewType, object viewModel)
        {
            object view = _container.Resolve(viewType);
            _logger.Debug($"Resolved to instance of '{view.GetType()}'.");

            if (view is DependencyObject depObj)
            {
                ContainerPropertyExtention.SetContainer(depObj, _container);

                if (view is FrameworkElement frameworkElement)
                    frameworkElement.DataContext = viewModel;
            }

            InitializeComponent(view);

            return view;
        }

        private void InitializeComponent(object element)
        {
            MethodInfo method = element.GetType()
                                       .GetMethod("InitializeComponent", BindingFlags.Instance | BindingFlags.Public);

            method?.Invoke(element, null);
        }
    }
}