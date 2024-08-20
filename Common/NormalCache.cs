using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SML.Common
{
    public class NormalCache : IDisposable
    {
        Dictionary<string, object> _cache = [];
        public event Action<IEnumerable<KeyValuePair<string, object>>> OnDispose;
        ~NormalCache()
        {
            if (_cache.Count > 0)
            {
                OnDispose?.Invoke(_cache);
                _cache.Clear();
                _cache = null;
            }
        }
        public void Add(string key, object value)
        {
            _cache.Add(key, value);
        }
        public bool TryAdd(string key, object value)
        {
            return _cache.TryAdd(key, value);
        }
        public bool Remove(string key, out object value)
        {
            return _cache.Remove(key, out value);
        }
        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default;
        }
        public bool TryGet<T>(string key, out T obj)
        {
            if (_cache.TryGetValue(key, out var value) && value is T t)
            {
                obj = t;
                return true;
            }
            obj = default;
            return false;
        }
        public void Clear()
        {
            _cache.Clear();
        }
        public void Dispose()
        {
            if (_cache.Count > 0)
            {
                OnDispose?.Invoke(_cache);
                _cache.Clear();
                _cache = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
