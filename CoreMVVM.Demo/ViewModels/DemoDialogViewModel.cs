using CoreMVVM.Demo.Validation;
using CoreMVVM.Windows;
using System.Windows.Input;

namespace CoreMVVM.Demo.ViewModels
{
    public class DemoDialogViewModel : DialogBase<string>
    {
        public DemoDialogViewModel()
        {
            CloseCommand = new RelayCommand<string>(OnClose);
            ValidateAllCommand = new RelayCommand(OnValidateAll);
        }

        #region Commands

        #region Close

        public ICommand CloseCommand { get; }

        private void OnClose(string input)
        {
            Close(input);
        }

        #endregion Close

        #region ValidateAll

        public ICommand ValidateAllCommand { get; }

        private void OnValidateAll()
        {
            ValidateAllProperties();
        }

        #endregion ValidateAll

        #endregion Commands

        #region Properties

        private string _username;
        private string _fullname;

        [UsernameValidation]
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        [UsernameValidation]
        public string Fullname
        {
            get => _fullname;
            set => SetProperty(ref _fullname, value);
        }

        #endregion Properties
    }
}