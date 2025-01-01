using SML.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SML.ECS_DOT
{
    internal class ArcheType
    {
        static Dictionary<int, ArcheType> _archeTypes = [];
        Dictionary<Type, int> _componentOffsets = [];
        List<IntPtr> _chunks = [];
        int _countPerChunk;
        int _size;
        private static int GenerateHashKey(Type[] types)
        {
            unchecked
            {
                int hash = 17;
                foreach (Type type in types)
                {
                    hash = hash * 23 + type.MetadataToken;
                    hash = hash * 23 + type.Module.GetHashCode();
                }
                return hash;
            }
        }
        internal static ArcheType GetOrCreate(Type[] components)
        {
            int hash = GenerateHashKey(components);
            if (_archeTypes.TryGetValue(hash, out var archeType))
            {
                return archeType;
            }
            int totalSize = sizeof(int);
            Dictionary<Type, int> componentSizes = [];
            foreach (var component in components)
            {
                componentSizes[component] = totalSize;
                int componentSize = Marshal.SizeOf(component);
                totalSize += componentSize;
            }
            int countPerChunk = 65536 / totalSize;
            archeType = new ArcheType
            {
                _componentOffsets = componentSizes,
                _countPerChunk = countPerChunk,
                _size = totalSize
            };
            IntPtr ptr = Marshal.AllocHGlobal(countPerChunk * totalSize);
            archeType._chunks.Add(ptr);
            _archeTypes[hash] = archeType;
            return archeType;
        }
        internal bool TryGetComponent<T>(int entityID, out IntPtr componentPtr) where T : struct, IComponentData
        {
            if (_componentOffsets.TryGetValue(typeof(T), out var offset))
            {
                foreach (var chunk in _chunks)
                {
                    for (int i = 0; i < _countPerChunk; i++)
                    {
                        int _entityID = Marshal.PtrToStructure<int>(chunk + i * _size);
                        if (entityID == _entityID)
                        {
                            componentPtr = chunk + i * _size + offset;
                            return true;
                        }
                    }
                }
            }
            componentPtr = IntPtr.Zero;
            return false;
        }
        internal void DestoryEntity(int entityID)
        {
            foreach (var chunk in _chunks)
            {
                for (int i = 0; i < _countPerChunk; i++)
                {
                    int _entityID = Marshal.PtrToStructure<int>(chunk + i * _size);
                    if (entityID == _entityID)
                    {
                        Marshal.Copy(new byte[_size], 0, chunk + i * _size, _size);
                    }
                }
            }
        }
        internal void CreateEntity(int entityID)
        {
            foreach (var chunk in _chunks)
            {
                for (int i = 0; i < _countPerChunk; i++)
                {
                    if (Marshal.PtrToStructure<int>(chunk + i * _size) == 0)
                    {
                    }
                }
            }
        }
    }
}
