using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConfigManager.Entity
{
    public class Package : IElement
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [JsonProperty("dep", NullValueHandling = NullValueHandling.Include)]
        public List<string> Dependencies;
    }
}
