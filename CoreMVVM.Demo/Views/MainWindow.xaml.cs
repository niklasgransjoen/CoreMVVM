using System;
using System.Windows;

namespace CoreMVVM.Demo.Views
{
    public partial class MainWindow : Window
    {
        private readonly ILogger _logger;

        public MainWindow(ILogger logger)
        {
            _logger = logger;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ((ScreenPrinter)_logger).RegisterTextBox(textBox);
        }

        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            _logger.Debug("Debug action performed. Result: success.");
        }

        private void Log_Click(object sender, RoutedEventArgs e)
        {
            _logger.Log("That button click was kind of important, and has been logged.");
        }

        private void Error_Click(object sender, RoutedEventArgs e)
        {
            _logger.Error("Clicking that button sent me into a state I shouldn't have been in. That's called an error.");
        }

        private void ExceptionClick(object sender, RoutedEventArgs e)
        {
            _logger.Exception("An exception was thrown by clicking that button!", new Exception("Don't click the exception button, it will throw exceptions!"));
        }
    }
}