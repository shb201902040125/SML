using Microsoft.Xna.Framework;

namespace SML.Common
{
    public static class ThreadHelper
    {
        public static bool IsMainThread => ThreadCheck.IsMainThread;
        public static void CheckMainThread() => ThreadCheck.CheckThread();
    }
}
