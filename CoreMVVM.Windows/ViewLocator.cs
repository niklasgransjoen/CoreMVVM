using System;
using System.Reflection;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Provides methods for retrieving a view instance from a given view model.
    /// </summary>
    public class ViewLocator : IViewLocator
    {
        private readonly IContainer container;
        private readonly ILogger logger;

        /// <summary>
        /// Returns the type of a view based on the type of the view model.
        /// </summary>
        public Func<Type, Type> GetViewTypeFromViewModelType;

        /// <summary>
        /// Returns the name of a view based on the name of a view model.
        /// </summary>
        public Func<string, string> GetViewTypeNameFromViewModelTypeName;

        public ViewLocator(IContainer container, ILogger logger)
        {
            this.container = container;
            this.logger = logger;

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

        /// <summary>
        /// Gets the view for the view model of a given type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        public object GetView<TViewModel>()
        {
            TViewModel viewModel = container.Resolve<TViewModel>();
            return GetView(viewModel);
        }

        /// <summary>
        /// Gets the view for the given view model.
        /// </summary>
        public object GetView(object viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            logger.Debug($"View for view model '{viewModel.GetType()} requested.");
            Type viewType = GetViewTypeFromViewModelType(viewModel.GetType());
            if (viewType == null)
            {
                logger.Error($"Failed to find view for view model of type '{viewModel.GetType()}'.");
                throw new InvalidOperationException($"No view found for view model of type '{viewModel.GetType()}'.");
            }

            object view = container.Resolve(viewType);
            if (view is FrameworkElement frameworkElement)
                frameworkElement.DataContext = viewModel;

            InitializeComponent(view);

            return view;
        }

        private static void InitializeComponent(object element)
        {
            var method = element.GetType().GetMethod("InitializeComponent", BindingFlags.Instance | BindingFlags.Public);
            method?.Invoke(element, null);
        }
    }
}