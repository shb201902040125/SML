using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SML.Common
{
    public static class ThreadHelper
    {
        public static bool IsMainThread => ThreadCheck.IsMainThread;
        public static void CheckMainThread() => ThreadCheck.CheckThread();
    }
}
