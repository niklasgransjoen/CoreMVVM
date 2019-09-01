using System;

namespace CoreMVVM.Tests
{
    internal interface IInterface { }

    internal class Class { }

    internal struct Struct { }

    internal class Implementation : IInterface { }

    internal interface IDisposableInterface : IDisposable
    {
        bool IsDisposed { get; }
    }

    internal class Disposable : IDisposableInterface
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    internal class DisposableSingleton : IDisposableInterface
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    internal class DisposableLifetimeScopedResource : IDisposableInterface
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}