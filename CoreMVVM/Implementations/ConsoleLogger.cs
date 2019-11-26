using System;

namespace CoreMVVM.Implementations
{
    /// <summary>
    /// Logs events and exceptions to the console.
    /// </summary>
    public sealed class ConsoleLogger : ILogger
    {
        #region Methods

        /// <summary>
        /// Logs an event for debug purposes.
        /// </summary>
        /// <param name="event">The event to log.</param>
        public void Debug(string @event)
        {
            Log(@event);
        }

        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="event">The event to log.</param>
        public void Log(string @event)
        {
            Console.ResetColor();

            WriteDateTime();
            Console.WriteLine(@event);
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="error">The error to log.</param>
        public void Error(string error)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;

            WriteDateTime();
            Console.WriteLine(error);

            Console.ResetColor();
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="message">A message describing the circumstance of the exception.</param>
        /// <param name="e">The exception to log.</param>
        public void Exception(string message, Exception e)
        {
            Error(message);

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e);

            Console.ResetColor();
        }

        #endregion Methods

        #region Private methods

        private void WriteDateTime()
        {
            Console.Write(DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss"));
            Console.Write(" - ");
        }

        #endregion Private methods
    }
}