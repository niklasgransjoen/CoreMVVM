using CoreMVVM.Demo.Views;
using CoreMVVM.IOC;
using System.Threading.Tasks;

namespace CoreMVVM.Demo.ViewModels
{
    internal class MainWindowModel : BaseModel
    {
        public MainWindowModel(SinglePageViewModel content)
        {
            Content = content;
        }

        private object _content;

        public object Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
    }
}