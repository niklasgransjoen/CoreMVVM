using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// For presenting view models with their registered view.
    /// </summary>
    public class ViewModelPresenter : FrameworkElement
    {
        private readonly Lazy<IViewLocator> _viewLocator = ContainerProvider.ResolveRequiredService<Lazy<IViewLocator>>();
        private readonly Dictionary<Type, FrameworkElement> _cachedViews = new Dictionary<Type, FrameworkElement>();

        static ViewModelPresenter()
        {
            FocusableProperty.OverrideMetadata(typeof(ViewModelPresenter),
                new FrameworkPropertyMetadata(defaultValue: false));
        }

        #region Dependency properties

        #region ViewModel

        /// <summary>
        /// The dependency property for the bindable view model.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(object), typeof(ViewModelPresenter),
                new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.AffectsMeasure, OnViewModelChanged));

        /// <summary>
        /// Gets or sets the view model for which this control should display the corresponding view.
        /// </summary>
        public object ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        private static void OnViewModelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ViewModelPresenter presenter = (ViewModelPresenter)o;

            presenter.RemoveLogicalChild(e.OldValue);
            presenter.AddLogicalChild(e.NewValue);

            presenter.UpdateView();
        }

        #endregion ViewModel

        #region CacheViews

        /// <summary>
        /// Identifies the <see cref="CacheViews"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CacheViewsProperty =
            DependencyProperty.Register(nameof(CacheViews), typeof(bool), typeof(ViewModelPresenter),
                new PropertyMetadata(true, CacheViewsPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating if resolved views should be cached.
        /// </summary>
        /// <value>Default is true.</value>
        /// <remarks>
        /// Changing this value clears the cache collection.
        /// </remarks>
        public bool CacheViews
        {
            get => (bool)GetValue(CacheViewsProperty);
            set => SetValue(CacheViewsProperty, value);
        }

        private static void CacheViewsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var presenter = (ViewModelPresenter)o;
            presenter._cachedViews.Clear();
        }

        #endregion CacheViews

        #region CacheViewsWithDataContext

        /// <summary>
        /// Identifies the <see cref="CacheViewsWithDataContext"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CacheViewsWithDataContextProperty =
            DependencyProperty.Register(nameof(CacheViewsWithDataContext), typeof(bool), typeof(ViewModelPresenter),
                new PropertyMetadata(false, CacheViewsWithDataContextPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating if views should be cached without clearing their <see cref="FrameworkElement.DataContext"/>.
        /// </summary>
        /// <value>Default is false.</value>
        /// <remarks>
        /// Clearing the DataContext property might be expensive depending on the view.
        /// - Definitely set this if the view model bound to the view is expected to stay the same.
        /// - Consider setting this even when the view model might change, if the view is complex.
        /// - The negative effects of setting this is that the cached view will keep the view model
        /// alive through a reference to it as its DataContext.
        ///
        /// Setting this false removes the DataContext of all cached views, and might be very expensive if the cache is big.
        /// </remarks>
        public bool CacheViewsWithDataContext
        {
            get => (bool)GetValue(CacheViewsWithDataContextProperty);
            set => SetValue(CacheViewsWithDataContextProperty, value);
        }

        private static void CacheViewsWithDataContextPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var presenter = (ViewModelPresenter)o;
            if (!presenter.CacheViewsWithDataContext)
            {
                foreach (var view in presenter._cachedViews.Values)
                {
                    ClearDataContext(view);
                }
            }
        }

        #endregion CacheViewsWithDataContext

        #endregion Dependency properties

        #region Properties

        private FrameworkElement _view;

        private FrameworkElement View
        {
            get => _view;
            set
            {
                if (_view == value)
                    return;

                // Clear the old view.
                RemoveVisualChild(_view);
                if (_view != null)
                {
                    if (CacheViews && !CacheViewsWithDataContext)
                    {
                        ClearDataContext(_view);
                    }
                }

                // Present new view.
                _view = value;
                AddVisualChild(_view);
            }
        }

        #endregion Properties

        protected override IEnumerator LogicalChildren => new[] { ViewModel }.GetEnumerator(); //new Enumerator(ViewModel);

        protected override int VisualChildrenCount => View is null ? 0 : 1;

        protected override Visual GetVisualChild(int index)
        {
            return _view;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (View is null)
                return Size.Empty;

            View.Measure(availableSize);
            return View.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (View is null)
                return finalSize;

            View.Arrange(new Rect(finalSize));
            return View.RenderSize;
        }

        private void UpdateView()
        {
            if (ViewModel is null)
            {
                View = null;
                return;
            }

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                View = new TextBlock(new Run(ViewModel.GetType().FullName));
                return;
            }

            View = ResolveView(ViewModel);
        }

        #region Utility

        /// <summary>
        /// Resolve the view the given view model.
        /// </summary>
        private FrameworkElement ResolveView(object viewModel)
        {
            if (!CacheViews)
            {
                return getView();
            }

            Type viewModelType = ViewModel.GetType();
            if (_cachedViews.TryGetValue(viewModelType, out FrameworkElement view))
            {
                view.DataContext = viewModel;
            }
            else
            {
                // Is it safe to assume the DataContext of the view is initialized when resolved?
                view = getView();
                _cachedViews[viewModelType] = view;
            }
            return view;

            FrameworkElement getView()
            {
                object rawView = _viewLocator.Value.ResolveView(viewModel);
                if (rawView is FrameworkElement frameworkElement)
                    return frameworkElement;

                throw new Exception($"Resolved view of type '{rawView.GetType()}' does not inherit from type '{typeof(FrameworkElement)}'.");
            }
        }

        private static void ClearDataContext(FrameworkElement element)
        {
            element.DataContext = null;
        }

        #endregion Utility
    }
}