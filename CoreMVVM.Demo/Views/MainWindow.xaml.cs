using CoreMVVM.IOC;
using System;
using System.Windows;

namespace CoreMVVM.Demo.Views
{
    [Scope(ComponentScope.Singleton)]
    public partial class MainWindow : Window
    {
        private readonly ScreenPrinter _screenPrinter;

        public MainWindow(ScreenPrinter screenPrinter)
        {
            _screenPrinter = screenPrinter;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _screenPrinter.RegisterTextBox(textBox);
        }
    }
}