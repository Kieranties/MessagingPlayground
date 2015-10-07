namespace ZMQ
{
    public class Options
    {
        public int Messages { get; set; } = 1000;

        public int Port { get; set; } = 5555;

        public string Protocol { get; set; } = "tcp";

        public string Host { get; set; } = "localhost";

        public string SocketType { get; set; }

        public string Id { get; set; } = System.Guid.NewGuid().ToString();
    }
}
