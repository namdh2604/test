using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Voltage.Witches.Models.Avatar
{
    public class JsonClothingConverter : JsonConverter
    {
        public override bool CanConvert(Type type)
        {
            return (type == typeof(AvatarManifest.AvatarItemEntry));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            return jsonObject.ToObject<AvatarManifest.AvatarItemEntry>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            AvatarManifest.AvatarItemEntry entry = (AvatarManifest.AvatarItemEntry)value;
            JObject obj = JObject.FromObject(entry);
            if (string.IsNullOrEmpty(entry.SubCategory))
            {
                obj.Remove("SubCategory");
            }

            serializer.Serialize(writer, obj);
        }
    }
}

