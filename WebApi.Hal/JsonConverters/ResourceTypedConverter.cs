#region

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace WebApi.Hal.JsonConverters
{
    public class ResourceTypedConverter : ResourceConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resourceValue = (Representation) value;
            AddSelfLink(resourceValue);


            var resourceType = resourceValue.GetType();
            var resourceProperties = resourceType.GetProperties();
            var modelValue = resourceProperties.First(p => p.IsModelProperty()).GetValue(resourceValue, null);

            var modelJson = JObject.FromObject(modelValue, serializer);
            var resourceJson = JObject.FromObject(resourceValue);

            //merge these guys
            foreach (var jProperty in resourceJson.Properties())
            {
                var propertyName = jProperty.Name;
                modelJson.Add(propertyName, resourceJson[propertyName]);
            }

            serializer.Serialize(writer, modelJson);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            throw new NotImplementedException("Typed representations are readonly!");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsResourceTyped();
        }
    }
}