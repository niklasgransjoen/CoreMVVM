using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Implementation of <see cref="IViewInitializer"/> that assigns the datacontext if the view is of type <see cref="FrameworkElement"/>.
    /// </summary>
    public class ViewDataContextInitializer : IViewInitializer
    {
        public virtual void InitView(object viewModel, object view)
        {
            if (view is FrameworkElement frameworkElement)
            {
                frameworkElement.DataContext = viewModel;
            }
        }
    }
}