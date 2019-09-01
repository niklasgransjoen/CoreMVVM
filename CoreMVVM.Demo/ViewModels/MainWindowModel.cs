using CoreMVVM.Windows;
using System;
using System.Windows.Input;

namespace CoreMVVM.Demo.ViewModels
{
    internal class MainWindowModel : BaseModel
    {
        private readonly ILogger _logger;
        private readonly IWindowManager _windowManager;

        public MainWindowModel(
            SinglePageViewModel content,
            ILogger logger,
            IWindowManager windowManager)
        {
            Content = content;
            _logger = logger;
            _windowManager = windowManager;

            DebugCommand = new RelayCommand(OnDebug);
            LogCommand = new RelayCommand(OnLog);
            ErrorCommand = new RelayCommand(OnError);
            ExceptionCommand = new RelayCommand(OnException);

            ShowDialogCommand = new RelayCommand(OnShowDialog);
        }

        #region Commands

        public ICommand DebugCommand { get; }
        public ICommand LogCommand { get; }
        public ICommand ErrorCommand { get; }
        public ICommand ExceptionCommand { get; }

        private void OnDebug()
        {
            _logger.Debug("Debug action performed. Result: success.");
        }

        private void OnLog()
        {
            _logger.Log("That button click was kind of important, and has been logged.");
        }

        private void OnError()
        {
            _logger.Error("Clicking that button sent me into a state I shouldn't have been in. That's called an error.");
        }

        private void OnException()
        {
            _logger.Exception("An exception was thrown by clicking that button!", new Exception("Don't click the exception button, it will throw exceptions!"));
        }

        #region ShowDialog

        public ICommand ShowDialogCommand { get; }

        private void OnShowDialog()
        {
            _windowManager.ShowDialog<DialogWindowModel>();
        }

        #endregion ShowDialog

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