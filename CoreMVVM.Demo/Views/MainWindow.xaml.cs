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

        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            _screenPrinter.Debug("Debug action performed. Result: success.");
        }

        private void Log_Click(object sender, RoutedEventArgs e)
        {
            _screenPrinter.Log("That button click was kind of important, and has been logged.");
        }

        private void Error_Click(object sender, RoutedEventArgs e)
        {
            _screenPrinter.Error("Clicking that button sent me into a state I shouldn't have been in. That's called an error.");
        }

        private void ExceptionClick(object sender, RoutedEventArgs e)
        {
            _screenPrinter.Exception("An exception was thrown by clicking that button!", new Exception("Don't click the exception button, it will throw exceptions!"));
        }
    }
}