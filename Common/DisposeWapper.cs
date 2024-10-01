using System;

namespace SML.Common
{
    public class DisposeWapper<T> : IDisposable
    {
        private Action<T> _dispose;
        public T Value { get; private set; }
        public bool Disposed { get; private set; }
        public DisposeWapper(T value, Action<T> dispose)
        {
            Value = value;
            _dispose = dispose;
        }
        ~DisposeWapper()
        {
            if (Disposed)
            {
                return;
            }
            _dispose?.Invoke(this);
            _dispose = null;
            Value = default;
        }
        public void Dispose()
        {
            if(Disposed)
            {
                return;
            }
            _dispose?.Invoke(Value);
            _dispose = null;
            Value = default;
            Disposed = true;
            GC.SuppressFinalize(this);
        }
        public static implicit operator T(DisposeWapper<T> wapper)
        {
            return wapper.Value;
        }
    }
}
