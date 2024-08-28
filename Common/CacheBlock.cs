using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SML.Common
{
    public unsafe class CacheBlock<T> : IDisposable where T : struct
    {
        private T[] _data;
        private GCHandle _handle;
        private SegmentTreeAllocator _allocator;
        private HashSet<Interface> undisposedInterfaces = [];
        internal object _lock = new();
        public CacheBlock(int capacity)
        {
            _data = new T[capacity];
            _handle = GCHandle.Alloc(_data, GCHandleType.Pinned);
            _allocator = new SegmentTreeAllocator(capacity);
        }
        ~CacheBlock()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_data != null)
                {
                    List<Interface> list = new(undisposedInterfaces);
                    foreach (Interface @interface in list)
                    {
                        @interface.Dispose();
                    }
                    _handle.Free();
                    _data = null;
                    _allocator = null;
                }
            }
        }
        public bool Rent(int size, out Interface @interface, SegmentTreeAllocator.AllocType allocType = SegmentTreeAllocator.AllocType.FirstFit)
        {
            lock (_lock)
            {
                int pos = _allocator.Rent(size, allocType);
                if (pos < 0)
                {
                    @interface = null;
                    return false;
                }
                else
                {
                    @interface = new Interface(this, pos, size);
                    undisposedInterfaces.Add(@interface);
                    return true;
                }
            }
        }
        public class Interface : IDisposable
        {
            private CacheBlock<T> _cache;
            private int _pos;
            private int _size;
            internal Interface(CacheBlock<T> cache, int pos, int size)
            {
                _cache = cache;
                _pos = pos;
                _size = size;
            }
            public ref T this[int index]
            {
                get
                {
                    return ref _cache._data[_pos + index];
                }
            }
            public void Clear()
            {
                for (int i = _pos; i < _pos + _size; i++)
                {
                    this[i] = default;
                }
            }
            public Span<T> AsSpan()
            {
                return new Span<T>(_cache._data, _pos, _size);
            }
            ~Interface()
            {
                Dispose(false);
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    lock (_cache._lock)
                    {
                        _cache._allocator.Return(_pos, _size);
                        _cache.undisposedInterfaces.Remove(this);
                        _cache = null;
                    }
                }
            }
        }
    }

}