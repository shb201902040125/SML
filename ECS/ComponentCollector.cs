using SML.Common;
using SML.Net;
using System;
using System.Reflection;

namespace SML.ECS
{
    internal static class ComponentCollector<T> where T : struct
    {
        private static SparseSet<int, T> _components = new(16);
        public static ref T GetOrCreate(Entity entity) => ref GetOrCreate(entity.UniqueID);
        internal static ref T GetOrCreate(int uniqueID)
        {
            if(_components.Contains(uniqueID))
            {
                return ref _components[uniqueID];
            }
            if (_components.IsFull)
            {
                _components.Resize(_components.Capacity * 2);
            }
            _components.Add(uniqueID, new T());
            if (typeof(T) != typeof(OnEntityDestroy))
            {
                ComponentCollector<OnEntityDestroy>.GetOrCreate(uniqueID).OnDestory += Destory;
            }
            return ref _components[uniqueID];
        }
        internal static void Destory(Entity entity)
        {
            _components.Remove(entity.UniqueID);
        }
    }
}
