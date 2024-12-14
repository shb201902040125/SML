using SML.Common;

namespace SML.Net
{
    public abstract class Socket
    {
        public RemoteAddress RemoteAddress { get; protected set; }

        private CacheBlock<byte> buffer;
        public bool TryGetBuffer(int size, out CacheBlock<byte>.Interface @interface)
        {
            if (buffer?.Rent(size, out @interface) ?? false)
            {
                return true;
            }
            @interface = null;
            return false;
        }
    }
}
