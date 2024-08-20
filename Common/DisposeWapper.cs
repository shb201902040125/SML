using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SML.Common
{
    public class DisposeWapper<T> : IDisposable
    {
        T _value;
        Action<T> _dispose;
        public T Value => _value;
        public bool Disposed { get; private set; }
        public DisposeWapper(T value, Action<T> dispose)
        {
            _value = value;
            _dispose = dispose;
        }
        ~DisposeWapper()
        {
            if(!Disposed)
            {
                _dispose?.Invoke(this);
            }
        }
        public void Dispose()
        {
            _dispose?.Invoke(_value);
            Disposed = true;
            GC.SuppressFinalize(this);
        }
        public static implicit operator T(DisposeWapper<T> wapper)
        {
            return wapper._value;
        }
    }
}
