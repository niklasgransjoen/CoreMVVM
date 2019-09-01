using System;
using System.Windows;

namespace CoreMVVM.Demo.Views
{
    public partial class MainWindow : Window
    {
        private readonly ScreenPrinter _screenPrinter;

        public MainWindow(ScreenPrinter screenPrinter)
        {
            _screenPrinter = screenPrinter;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _screenPrinter.RegisterTextBox(textBox);
        }
    }
}