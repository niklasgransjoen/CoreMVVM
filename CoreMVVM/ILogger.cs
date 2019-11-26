using CoreMVVM.Implementations;
using CoreMVVM.IOC;
using System;

namespace CoreMVVM
{
    /// <summary>
    /// Implements logic for logging events and exceptions.
    /// </summary>
    [FallbackImplementation(typeof(ConsoleLogger))]
    public interface ILogger
    {
        /// <summary>
        /// Logs an event for debug purposes.
        /// </summary>
        /// <param name="event">The event to log.</param>
        void Debug(string @event);

        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="event">The event to log.</param>
        void Log(string @event);

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="error">The error to log.</param>
        void Error(string error);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="message">A message describing the circumstance of the exception.</param>
        /// <param name="e">The exception to log.</param>
        void Exception(string message, Exception e);
    }
}