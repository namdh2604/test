using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace Voltage.Witches.User
{

    // Takes care of converting the player data object to and from a JSON string
    public class JSONPlayerDataSerializer : IPlayerDataSerializer
    {
        private readonly JsonSerializer _serializer;
        private readonly JsonSerializer _deserializer;

        public JSONPlayerDataSerializer()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new IsoDateTimeConverter());
            _serializer = JsonSerializer.Create(settings);
            _deserializer = new JsonSerializer();
        }

        public string Serialize(PlayerDataStore playerData, bool prettyPrint=false)
        {
            string result;

            using (StringWriter sw = new StringWriter())
            {
                using (JsonWriter jsonWriter = new JsonTextWriter(sw))
                {
                    if (prettyPrint)
                    {
                        jsonWriter.Formatting = Formatting.Indented;
                    }
                    _serializer.Serialize(jsonWriter, playerData);
                }

                result = sw.ToString();
            }

            return result;
        }

        public PlayerDataStore Deserialize(string rawData)
        {
            PlayerDataStore result = null;

            using (StringReader sr = new StringReader(rawData))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(sr))
                {
                    result = _deserializer.Deserialize<PlayerDataStore>(jsonReader);
                }
            }

            return result;
        }
    }
}

