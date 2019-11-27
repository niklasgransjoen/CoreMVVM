using System;

namespace CoreMVVM
{
    public static class LoggerHelper
    {
        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(() => ContainerProvider.Resolve<ILogger>());

        #region ILogger

        /// <summary>
        /// Logs an event for debug purposes.
        /// </summary>
        /// <param name="event">The event to log.</param>
        public static void Debug(string @event) => _logger.Value.Debug(@event);

        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="event">The event to log.</param>
        public static void Log(string @event) => _logger.Value.Log(@event);

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="error">The error to log.</param>
        public static void Error(string error) => _logger.Value.Error(error);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="message">A message describing the circumstance of the exception.</param>
        /// <param name="e">The exception to log.</param>
        public static void Exception(string message, Exception e) => _logger.Value.Exception(message, e);

        #endregion ILogger
    }
}