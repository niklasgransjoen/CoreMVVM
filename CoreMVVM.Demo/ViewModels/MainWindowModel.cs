using CoreMVVM.IOC;
using CoreMVVM.Windows;
using System;
using System.Windows;
using System.Windows.Input;

namespace CoreMVVM.Demo.ViewModels
{
    [Scope(ComponentScope.Singleton)]
    internal class MainWindowModel : BaseModel
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IWindowManager _windowManager;

        public MainWindowModel(
            SinglePageViewModel content,
            ILifetimeScope lifetimeScope,
            IWindowManager windowManager)
        {
            Content = content;
            _lifetimeScope = lifetimeScope;
            _windowManager = windowManager;

            DebugCommand = new RelayCommand(OnDebug);
            LogCommand = new RelayCommand(OnLog);
            ErrorCommand = new RelayCommand(OnError);
            ExceptionCommand = new RelayCommand(OnException);

            ShowDialog1Command = new RelayCommand(OnShowDialog1);
            ShowDialog2Command = new RelayCommand(OnShowDialog2, CanShowDialog2);
        }

        #region Commands

        public ICommand DebugCommand { get; }
        public ICommand LogCommand { get; }
        public ICommand ErrorCommand { get; }
        public ICommand ExceptionCommand { get; }

        private void OnDebug()
        {
            LoggerHelper.Debug("Debug action performed. Result: success.");
        }

        private void OnLog()
        {
            LoggerHelper.Log("That button click was kind of important, and has been logged.");
        }

        private void OnError()
        {
            LoggerHelper.Error("Clicking that button sent me into a state I shouldn't have been in. That's called an error.");
        }

        private void OnException()
        {
            LoggerHelper.Exception("An exception was thrown by clicking that button!", new Exception("Don't click the exception button, it will throw exceptions!"));
        }

        #region ShowDialog 1

        public ICommand ShowDialog1Command { get; }

        private void OnShowDialog1()
        {
            _windowManager.ShowDialog<DialogWindowModel>();
        }

        #endregion ShowDialog 1

        #region ShowDialog 2

        private bool _isShowingDialog;

        public RelayCommand ShowDialog2Command { get; }

        private bool CanShowDialog2() => !_isShowingDialog;

        private async void OnShowDialog2()
        {
            _isShowingDialog = true;
            ShowDialog2Command.RaiseCanExecute();

            var dialog = _lifetimeScope.Resolve<DemoDialogViewModel>();
            Window dialogView = _windowManager.ShowWindow(dialog);

            string result = await dialog.Task;
            dialogView.Close();

            MessageBox.Show($"You entered: {result}", "Dialog result");

            _isShowingDialog = false;
            ShowDialog2Command.RaiseCanExecute();
        }

        #endregion ShowDialog 2

        #endregion Commands

        #region Properties

        private object _content;

        public object Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        #endregion Properties
    }
}