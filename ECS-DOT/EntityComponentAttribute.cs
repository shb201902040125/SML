using SML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SML.ECS_DOT
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class EntityComponentAttribute : Attribute
    {
        internal Type[] checkdComponentTypes;
        public EntityComponentAttribute(params Type[] types)
        {
            HashSet<Type> typesSet = [];
            foreach (Type type in types)
            {
                if(type is IComponentData&& CommonHelper.IsStruct(type))
                {
                    if(!typesSet.Add(type))
                    {
                        throw new ArgumentException("A type can be added only once");
                    }
                }
                else
                {
                    throw new ArgumentException("Only struct that inherit IComponentData are accepted");
                }
            }
            checkdComponentTypes = [.. typesSet.OrderBy(t => t.FullName)];
        }
    }
}
