using CoreMVVM.IOC;
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
            if (DesignerProperties.GetIsInDesignMode(o))
                return;

            ViewModelPresenter presenter = (ViewModelPresenter)o;
            presenter.UpdateView();
        }

        #endregion ViewModel

        #endregion Dependency properties

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            UpdateView();
        }

        private void UpdateView()
        {
            if (ViewModel != null)
            {
                LifetimeScopePropertyExtention.TryFindLifetimeScope(this, out ILifetimeScope lifetimeScope);
                if (lifetimeScope != null)
                    Content = lifetimeScope.Resolve<IViewLocator>().GetView(ViewModel);
                else
                    Content = ViewModel;
            }
            else
            {
                Content = null;
            }
        }
    }
}