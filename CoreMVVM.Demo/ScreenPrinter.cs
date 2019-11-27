using CoreMVVM.IOC;
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

        public void Debug(string @event)
        {
            AppendParagraph(@event, Brushes.Blue);
        }

        public void Log(string @event)
        {
            AppendParagraph(@event);
        }

        public void Error(string error)
        {
            AppendParagraph(error, Brushes.DarkRed);
        }

        public void Exception(string message, Exception e)
        {
            AppendParagraph(message, Brushes.DarkRed);
            AppendParagraph(e.ToString());
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
            string formattedMessage = DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss") + " - " + message;

            Run run = new Run(formattedMessage);
            if (foreground != null)
                run.Foreground = foreground;

            _paragraph.Inlines.Add(run);
            _paragraph.Inlines.Add(new LineBreak());
        }
    }
}