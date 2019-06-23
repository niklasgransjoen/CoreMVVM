using System;

namespace CoreMVVM
{
    /// <summary>
    /// An extention of <see cref="IDisposable"/>.
    /// </summary>
    public interface IAmDisposable : IDisposable
    {
        /// <summary>
        /// Gets a value indicating if this object has been disposed.
        /// </summary>
        bool IsDisposed { get; }
    }
}