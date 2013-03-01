#region

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            var resourceType = resource.GetType();

            if (resourceType.IsResourceTyped())
                WriteJsonTyped(resourceType, resource, writer, serializer);
            else
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
            return objectType.IsResource() && !objectType.IsResourceList();
        }

        protected void AddSelfLink(Representation resource)
        {
            resource.Links.Insert(0, new Link
                {
                    Rel = "self",
                    Href = resource.Href,
                    Title = resource.LinkTitle
                });
        }

        private static void WriteJsonTyped(Type resourceType, Representation resourceValue, JsonWriter writer,
                                           JsonSerializer serializer)
        {
            var resourceProperties = resourceType.GetProperties();
            var modelValue = resourceProperties.First(p => p.IsModelProperty()).GetValue(resourceValue, null);

            var modelJson = JObject.FromObject(modelValue, serializer);
            var resourceJson = JObject.FromObject(resourceValue, serializer);

            //merge these guys
            foreach (var jProperty in resourceJson.Properties())
            {
                var propertyName = jProperty.Name;
                modelJson.Add(propertyName, resourceJson[propertyName]);
            }

            serializer.Serialize(writer, modelJson);
        }
    }
}