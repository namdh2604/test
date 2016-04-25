using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Voltage.Witches.Models.Avatar
{
    public class JsonRectConverter : JsonConverter
    {

        public override bool CanConvert(Type type)
        {
            return (type == typeof(Rect));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            float x = jsonObject.Value<float>("x");
            float y = jsonObject.Value<float>("y");

            float width = jsonObject.Value<float>("width");
            float height = jsonObject.Value<float>("height");

            return new Rect(x, y, width, height);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Rect rect = (Rect)value;
            writer.WriteStartObject();

            writer.WritePropertyName("x");
            writer.WriteValue(rect.x);

            writer.WritePropertyName("y");
            writer.WriteValue(rect.y);

            writer.WritePropertyName("width");
            writer.WriteValue(rect.width);

            writer.WritePropertyName("height");
            writer.WriteValue(rect.height);

            writer.WriteEndObject();
        }

    }
}

