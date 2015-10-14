using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace POC.Messaging
{
    public class Message
    {   
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects, Formatting = Formatting.None };

        public T BodyAs<T>() => (T)Body;

        public object Body { get; set; }
        
        public IMessageQueueConnection ResponseConnection { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this, _serializerSettings);

        public Stream ToJsonStream() => new MemoryStream(Encoding.Default.GetBytes(ToJson()));

        public static Message FromJson(string json) => JsonConvert.DeserializeObject<Message>(json, _serializerSettings);

        public static Message FromJson(Stream stream) => FromJson(new StreamReader(stream).ReadToEnd());
    }
}
