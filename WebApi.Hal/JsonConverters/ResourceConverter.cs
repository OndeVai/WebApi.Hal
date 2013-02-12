#region

using System;
using Newtonsoft.Json;
using WebApi.Hal.Interfaces;

#endregion

namespace WebApi.Hal.JsonConverters
{
    public class ResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resource = (Representation) value;

            AddSelfLink(resource);

            serializer.Converters.Remove(this);
            serializer.Serialize(writer, resource);
            serializer.Converters.Add(this);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsResource() && !objectType.IsResourceList() && !objectType.IsResourceTyped();
        }

        protected bool IsResourceList(Type objectType)
        {
            return typeof (IRepresentationList).IsAssignableFrom(objectType);
        }

        protected void AddSelfLink(Representation resource)
        {
            resource.Links.Insert(0, new Link
            {
                Rel = "self",
                Href = resource.Href
            });
        }
    }
}