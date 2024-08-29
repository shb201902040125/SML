using System;
using System.Collections;
using System.Collections.Generic;

namespace SML.Common
{
    public class SparseSet<TKey, TValue> : IEnumerable<TValue> where TValue : struct
    {
        private TValue[] _datas;
        private Dictionary<TKey, int> _map;
        private Dictionary<int, TKey> _keys;
        private int _ptr = 0;
        public IEnumerable<TKey> Keys => _map.Keys;
        public SparseSet(int capacity)
        {
            _datas = new TValue[capacity];
            _map = [];
            _keys = [];
        }
        public bool Add(TKey key, TValue value)
        {
            if (_ptr >= _datas.Length)
            {
                return false;
            }
            _datas[_ptr] = value;
            _map[key] = _ptr;
            _keys[_ptr] = key;
            _ptr++;
            return true;
        }
        public bool Remove(TKey key)
        {
            if (!_map.Remove(key, out int offset))
            {
                return false;
            }
            _keys.Remove(offset);
            if (offset == _ptr - 1)
            {
                return true;
            }
            _datas[offset] = _datas[_ptr - 1];
            _map[_keys[_ptr - 1]] = offset;
            _ptr--;
            return true;
        }
        public bool Contains(TKey key)
        {
            return _map.ContainsKey(key);
        }
        public void Clear()
        {
            _map.Clear();
            _keys.Clear();
            _ptr = 0;
        }
        public IEnumerator<TValue> GetEnumerator()
        {
            for (int i = 0; i < _ptr; i++)
            {
                yield return _datas[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public ref TValue this[TKey key] => ref _datas[_map[key]];
        public Span<TValue> AsSpan()
        {
            return new Span<TValue>(_datas, 0, _ptr);
        }
    }
}
