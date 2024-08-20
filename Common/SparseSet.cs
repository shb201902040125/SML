﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SML.Common
{
    public class SparseSet<Tkey, TValue> : IEnumerable<TValue> where TValue : unmanaged
    {
        TValue[] _datas;
        Dictionary<Tkey, int> _map;
        Dictionary<int,Tkey> _keys;
        int _ptr = 0;
        public SparseSet(int capacity)
        {
            _datas = new TValue[capacity];
            _map = [];
            _keys = [];
        }
        public bool Add(Tkey key, TValue value)
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
        public bool Remove(Tkey key)
        {
            if (!_map.Remove(key, out var offset))
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
        public bool Contains(Tkey key) 
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
        public ref TValue this[Tkey key] => ref _datas[_map[key]];
        public Span<TValue> AsSpan()
        {
            return new Span<TValue>(_datas, 0, _ptr);
        }
    }
}
