using System.Windows.Markup;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Implementation of <see cref="IViewInitializer"/> that invokes <see cref="IComponentConnector.InitializeComponent"/> if the view implements <see cref="IComponentConnector"/>.
    /// </summary>
    public class ViewComponentInitializer : IViewInitializer
    {
        public virtual void InitView(object viewModel, object view)
        {
            if (view is IComponentConnector componentConnector)
                componentConnector.InitializeComponent();
        }
    }
}