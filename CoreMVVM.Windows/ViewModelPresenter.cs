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
        /// <summary>
        /// The dependency property for the bindable view model.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(object), typeof(ViewModelPresenter),
                new PropertyMetadata(default, OnViewModelChanged));

        /// <summary>
        /// The view model for which this control should display the corresponding view.
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
            if (e.NewValue != null)
            {
                IOC.IContainer container = ContainerPropertyExtention.GetContainer(o);
                var view = container.Resolve<IViewLocator>().GetView(e.NewValue);
                presenter.Content = view;
            }
            else
                presenter.Content = null;
        }
    }
}