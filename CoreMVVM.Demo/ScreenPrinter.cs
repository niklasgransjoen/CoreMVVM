using CoreMVVM.IOC;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CoreMVVM.Demo
{
    [Scope(ComponentScope.Singleton)]
    public class ScreenPrinter : ILogger
    {
        private readonly Paragraph _paragraph = new Paragraph();

        #region ILogger

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    AppendParagraph(message, Brushes.Blue);
                    break;

                case LogLevel.Information:
                    AppendParagraph(message);
                    break;

                case LogLevel.Warning:
                case LogLevel.Error:
                    AppendParagraph(message, Brushes.DarkRed);
                    break;
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state)
        {
            return Disposable.Instance;
        }

        #endregion ILogger

        #region Methods

        /// <summary>
        /// Registers a <see cref="RichTextBox"/> as the output source of this logger.
        /// </summary>
        public void RegisterTextBox(RichTextBox textBox)
        {
            if (textBox != null)
            {
                textBox.Document.Blocks.Clear();
                textBox.Document.Blocks.Add(_paragraph);
            }
        }

        #endregion Methods

        private void AppendParagraph(string message, Brush foreground = null)
        {
            string formattedMessage = DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss") + " - " + StringParser.Parse(message);

            Run run = new Run(formattedMessage);
            if (foreground != null)
                run.Foreground = foreground;

            _paragraph.Inlines.Add(run);
            _paragraph.Inlines.Add(new LineBreak());
        }

        #region Utility class

        private sealed class Disposable : IDisposable
        {
            public static IDisposable Instance { get; } = new Disposable();

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        #endregion Utility class
    }
}