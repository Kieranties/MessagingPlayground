using Newtonsoft.Json;
using System;
using System.IO;

namespace POC.Messages
{
    public static class StreamExtensions
    {
        public static string ReadToEnd(this Stream stream) => new StreamReader(stream).ReadToEnd();

        public static T FromJson<T>(this Stream stream) => JsonConvert.DeserializeObject<T>(stream.ReadToEnd());

        public static object FromJson(this Stream stream, Type type) => JsonConvert.DeserializeObject(stream.ReadToEnd(), type);

        public static object FromJson(this Stream stream, string messageType) => stream.FromJson(Type.GetType(messageType));
    }
}
