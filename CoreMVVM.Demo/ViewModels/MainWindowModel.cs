using CoreMVVM.IOC;
using CoreMVVM.Windows;
using CoreMVVM.Windows.Threading;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;

namespace CoreMVVM.Demo.ViewModels
{
    [Scope(ComponentScope.Singleton)]
    internal class MainWindowModel : BaseModel
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IResourceService _resourceService;
        private readonly IWindowManager _windowManager;

        public MainWindowModel(
            SinglePageViewModel content,
            ILifetimeScope lifetimeScope,
            IResourceService resourceService,
            IWindowManager windowManager)
        {
            Content = content;
            _lifetimeScope = lifetimeScope;
            _resourceService = resourceService;
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
            LoggerHelper.Debug("${res:DebugAction}");
        }

        private void OnLog()
        {
            LoggerHelper.Log("${res:LogAction}");
        }

        private void OnError()
        {
            LoggerHelper.Error("${res:ErrorAction}");
        }

        private void OnException()
        {
            LoggerHelper.Exception("${res:ExceptionAction}", new Exception("Don't click the exception button, it will throw exceptions!"));
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

            await TaskMaster.AwaitUIThread();
            dialogView.Close();

            string title = StringParser.GetResource("DialogResult");
            string body = StringParser.GetResource("YouEntered", new StringTagPair("result", result));
            MessageBox.Show(body, title);

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

        public IEnumerable Languages { get; } = new[] { "English (EU)", "Norwegian" };

        private string _selectedLanguage;

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (SetProperty(ref _selectedLanguage, value))
                {
                    string currentCulture = value.Equals("Norwegian") ? "no" : "en";
                    _resourceService.CurrentCulture = new System.Globalization.CultureInfo(currentCulture);
                }
            }
        }

        #endregion Properties
    }
}