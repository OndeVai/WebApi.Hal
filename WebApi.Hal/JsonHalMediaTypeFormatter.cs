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
        private readonly LinksConverter _linksConverter = new LinksConverter();
        private readonly ResourceConverter _resourceConverter = new ResourceConverter();
        private readonly ResourceListConverter _resourceListConverter = new ResourceListConverter();

        public JsonHalMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
            SerializerSettings.Converters.Add(_linksConverter);
            SerializerSettings.Converters.Add(_resourceListConverter);
            SerializerSettings.Converters.Add(_resourceConverter);
        }

        public override bool CanReadType(Type type)
        {
            return type.IsResource() && !type.IsResourceTyped();
        }

        public override bool CanWriteType(Type type)
        {
            return type.IsResource();
        }
    }
}