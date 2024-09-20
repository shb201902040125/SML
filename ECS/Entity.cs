using SML.Common;
using SML.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SML.ECS
{
    public struct Entity
    {
        private static BitArray IndexMap = new(16);
        private static SparseSet<int, Entity> _entities = new(16);
        public int UniqueID { get; private set; }
        public static ref Entity Create()
        {
            int uniqueID = IndexMap.FindFirstFalseIndex();
            if (uniqueID == -1)
            {
                IndexMap.Resize(IndexMap.Capacity * 2);
                uniqueID = IndexMap.FindFirstTrueIndex();
            }
            Entity entity = new()
            {
                UniqueID = uniqueID
            };
            if (!_entities.Add(uniqueID, entity))
            {
                _entities.Resize(_entities.Capacity * 2);
                _entities.Add(uniqueID, entity);
            }
            IndexMap[uniqueID] = true;
            return ref _entities[uniqueID];
        }
        public ref T GetOrCreateComponent<T>() where T : struct
        {
            return ref ComponentCollector<T>.GetOrCreate(UniqueID);
        }
        public void Destroy()
        {
            IndexMap[UniqueID] = false;
            ComponentCollector<OnEntityDestroy>.GetOrCreate(UniqueID).Invoke(this);
        }
        public override int GetHashCode()
        {
            return UniqueID;
        }
        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is Entity other && other.UniqueID == UniqueID;
        }
        public override string ToString()
        {
            return $"Entity:{UniqueID}";
        }
    }
    struct OnEntityDestroy
    {
        public event Action<Entity> OnDestory;
        public void Invoke(Entity entity)
        {
            OnDestory?.Invoke(entity);
        }
    }
}
