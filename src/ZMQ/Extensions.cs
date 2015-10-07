using NetMQ;

namespace ZMQ
{
    public static class Extensions
    {
        public static string Address(this Options options) => $"{options.Protocol}://{options.Host}:{options.Port}";
    }
}
