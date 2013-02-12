#region

using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using WebApi.Hal.JsonConverters;

#endregion

namespace WebApi.Hal
{
    public class JsonHalMediaTypeFormatter : JsonMediaTypeFormatter
    {
        readonly LinksConverter linksConverter = new LinksConverter();
        readonly ResourceConverter resourceConverter = new ResourceConverter();
        readonly ResourceListConverter resourceListConverter = new ResourceListConverter();
        readonly ResourceTypedConverter typedResourceConverter = new ResourceTypedConverter();

        public JsonHalMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
            SerializerSettings.Converters.Add(linksConverter);
            SerializerSettings.Converters.Add(resourceListConverter);
            SerializerSettings.Converters.Add(resourceConverter);
            SerializerSettings.Converters.Add(typedResourceConverter);
        }

        public override bool CanReadType(Type type)
        {
            return typeof (Representation).IsAssignableFrom(type);
        }

        public override bool CanWriteType(Type type)
        {
            return typeof (Representation).IsAssignableFrom(type);
        }
    }
}