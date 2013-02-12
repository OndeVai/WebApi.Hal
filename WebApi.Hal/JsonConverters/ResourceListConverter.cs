#region

using System;
using System.Linq;
using Newtonsoft.Json;
using WebApi.Hal.Interfaces;

#endregion

namespace WebApi.Hal.JsonConverters
{
    public class ResourceListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = (IRepresentationList) value;

            list.Links.Add(new Link
            {
                Rel = "self",
                Href = list.Href
            });

            writer.WriteStartObject();
            writer.WritePropertyName("_links");
            serializer.Serialize(writer, list.Links);

            writer.WritePropertyName("_embedded");
            writer.WriteStartObject();
            writer.WritePropertyName(list.Rel);
            writer.WriteStartArray();
            foreach (Representation halResource in list)
            {
                serializer.Serialize(writer, halResource);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();

            var listType = list.GetType();
            var propertyInfos = typeof (RepresentationList<>).GetProperties().Select(p => p.Name);
            foreach (var property in listType.GetProperties().Where(p => !propertyInfos.Contains(p.Name)))
            {
                writer.WritePropertyName(property.Name.ToLower());
                serializer.Serialize(writer, property.GetValue(value, null));
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsResource() && objectType.IsResourceList();
        }
    }
}