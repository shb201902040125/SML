using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SML.ECS_DOT
{
    internal class EntityManager
    {
        static Dictionary<Type, ArcheType> _entityArcheType = [];
        static Dictionary<Type, int> _nextID = [];
        public static void RegisterEntity<T>() where T : IEntity
        {
            var att = typeof(T).GetCustomAttribute<EntityComponentAttribute>() ?? throw new ArgumentException("This type lacks an EntityComponentAttribute");
            if (att.checkdComponentTypes.Length == 0)
            {
                throw new ArgumentException("Don't waste data space, okay?");
            }
            if (!_entityArcheType.ContainsKey(typeof(T)))
            {
                _entityArcheType[typeof(T)] = ArcheType.GetOrCreate(att.checkdComponentTypes);
            }
        }
        public static void SetEntityID<T>(T target) where T : IEntity
        {
            if (target.EntityID != 0)
            {
                _entityArcheType[typeof(T)].DestoryEntity(target.EntityID);
            }
            if(!_entityArcheType.TryGetValue(typeof(T), out var archeType))
            {
                throw new ArgumentException("Type of an unregistered entity");
            }
            if (!_nextID.TryGetValue(typeof(T), out var nextID))
            {
                _nextID[typeof(T)] = nextID = 1;
            }
            target.EntityID = nextID;
            _nextID[typeof(T)]++;

        }
    }
}
