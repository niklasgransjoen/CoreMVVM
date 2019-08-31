using System;

namespace CoreMVVM.Tests
{
    public interface IInterface { }

    public class Class { }

    public struct Struct { }

    public class Implementation : IInterface { }

    public interface IDisposableInterface : IDisposable
    {
        bool IsDisposed { get; }
    }

    public class Disposable : IDisposableInterface
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}