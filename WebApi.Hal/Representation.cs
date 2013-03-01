#region

using System.Collections.Generic;
using Newtonsoft.Json;
using WebApi.Hal.Interfaces;

#endregion

namespace WebApi.Hal
{
    public abstract class Representation<TModel> : Representation where TModel : class
    {
        [JsonIgnore]
        public TModel Model { get; set; }
    }

    public abstract class Representation : IResource
    {
        private bool creatingHyperMedia;
        private string href;
        private string linkName;
        private string rel;
        private bool selfLinkUpToDate;

        protected Representation()
        {
            Links = new HypermediaList(CreateHypermedia);
        }

        [JsonIgnore]
        public string Rel
        {
            get
            {
                // Prevent CreateHypermedia from being reentrant to this method
                if (creatingHyperMedia || selfLinkUpToDate)
                    return href;
                creatingHyperMedia = true;
                try
                {
                    CreateHypermedia();
                }
                finally
                {
                    creatingHyperMedia = false;
                }
                return rel;
            }
            set
            {
                rel = value;
                selfLinkUpToDate = false;
            }
        }

        [JsonIgnore]
        public string Href
        {
            get
            {
                // Prevent CreateHypermedia from being reentrant to this method
                if (creatingHyperMedia || selfLinkUpToDate)
                    return href;
                creatingHyperMedia = true;
                try
                {
                    CreateHypermedia();
                }
                finally
                {
                    creatingHyperMedia = false;
                }
                return href;
            }
            set
            {
                href = value;
                selfLinkUpToDate = false;
            }
        }

        [JsonIgnore]
        public string LinkName
        {
            get
            {
                // Prevent CreateHypermedia from being reentrant to this method
                if (creatingHyperMedia || selfLinkUpToDate)
                    return href;
                creatingHyperMedia = true;
                try
                {
                    CreateHypermedia();
                }
                finally
                {
                    creatingHyperMedia = false;
                }
                return linkName;
            }
            set
            {
                linkName = value;
                selfLinkUpToDate = false;
            }
        }

        [JsonIgnore]
        public string LinkTitle { get; set; }

        [JsonProperty("_links")]
        public IList<Link> Links { get; set; }

        

        protected internal abstract void CreateHypermedia();
    }
}