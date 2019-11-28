using CoreMVVM.Windows;
using System.Windows.Input;

namespace CoreMVVM.Demo
{
    /*
     * ViewModel not located in folder ViewModels.
     * Will still be resolved to DialogWindow.xaml because it's been registered with our IViewProvider implementation.
     */

    public class DialogWindowModel : BaseModel
    {
        public DialogWindowModel()
        {
            ButtonCommand = new RelayCommand<string>(OnButtonPressed);
        }

        #region Commands

        public ICommand ButtonCommand { get; }

        private void OnButtonPressed(string text)
        {
            Status = StringParser.GetResource("CurrentContent", new StringTagPair("text", text));
        }

        #endregion Commands

        #region Properties

        private string _status;

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        #endregion Properties
    }
}