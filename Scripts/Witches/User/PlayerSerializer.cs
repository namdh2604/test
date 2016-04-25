using Newtonsoft.Json;
using System.IO;

namespace Voltage.Story.User
{
    public class JSONSimpleSerializer : ISimpleSerializer
    {
        private readonly JsonSerializer _serializer;

        public JSONSimpleSerializer()
        {
            _serializer = new JsonSerializer();
        }

        public string Serialize(object obj)
        {
            string serialized = string.Empty;

            using (StringWriter sw = new StringWriter())
            {
                _serializer.Serialize(sw, obj);
                serialized = sw.ToString();
            }

            return serialized;
        }
    }
}

