using System;

namespace SML.Common
{
    public class DisposeWapper<T> : IDisposable
    {
        private Action<T> _dispose;
        public T Value { get; }
        public bool Disposed { get; private set; }
        public DisposeWapper(T value, Action<T> dispose)
        {
            Value = value;
            _dispose = dispose;
        }
        ~DisposeWapper()
        {
            if (!Disposed)
            {
                _dispose?.Invoke(this);
            }
        }
        public void Dispose()
        {
            _dispose?.Invoke(Value);
            Disposed = true;
            GC.SuppressFinalize(this);
        }
        public static implicit operator T(DisposeWapper<T> wapper)
        {
            return wapper.Value;
        }
    }
}
