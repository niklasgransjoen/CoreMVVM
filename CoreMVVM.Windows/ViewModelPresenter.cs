using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// For presenting view models with their registered view.
    /// </summary>
    public class ViewModelPresenter : ContentControl
    {
        private readonly Lazy<IViewLocator> _viewLocator = new Lazy<IViewLocator>(() => ContainerProvider.Resolve<IViewLocator>());

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
                new PropertyMetadata(default, OnViewModelChanged));

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
            presenter.UpdateView();
        }

        #endregion ViewModel

        #endregion Dependency properties

        private void UpdateView()
        {
            if (ViewModel is null)
            {
                Content = null;
                return;
            }

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Content = ViewModel.GetType().FullName;
                return;
            }

            // Ensure that the ViewModel has a corresponding DataTemplate.
            Type viewModelType = ViewModel.GetType();
            Type viewType = _viewLocator.Value.GetViewType(viewModelType);
            DataTemplateRegistrator.AssureDataTemplateExists(viewModelType, viewType);

            // Update the content.
            Content = ViewModel;
        }
    }
}