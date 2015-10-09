using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace POC.Messages
{
    public static class ObjectExtensions
    {
        public static string GetMessageType(this object obj) => obj.GetType().AssemblyQualifiedName;

        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj);

        public static Stream ToJsonStream(this object obj) => new MemoryStream(Encoding.Default.GetBytes(obj.ToJson()));
    }
}
