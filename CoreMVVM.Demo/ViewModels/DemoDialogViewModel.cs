using CoreMVVM.Windows;
using System.Windows.Input;

namespace CoreMVVM.Demo.ViewModels
{
    public class DemoDialogViewModel : DialogBase<string>
    {
        public DemoDialogViewModel()
        {
            CloseCommand = new RelayCommand<string>(OnClose);
        }

        #region Commands

        public ICommand CloseCommand { get; }

        private void OnClose(string input)
        {
            Close(input);
        }

        #endregion Commands
    }
}