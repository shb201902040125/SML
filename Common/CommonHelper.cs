using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SML.Common
{
    internal class CommonHelper
    {
        internal static bool IsStruct(Type type)
        {
            return type.IsValueType && !(type.IsPrimitive || type.IsEnum);
        }
    }
}
