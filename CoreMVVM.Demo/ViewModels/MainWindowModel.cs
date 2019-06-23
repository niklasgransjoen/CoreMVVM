using CoreMVVM.Demo.Views;

namespace CoreMVVM.Demo.ViewModels
{
    internal class MainWindowModel : BaseModel
    {
        public MainWindowModel(SinglePageViewModel content)
        {
            Content = content;
        }

        private object content;

        public object Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }
    }
}