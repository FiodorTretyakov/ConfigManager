using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConfigManager.Entity
{
    public class CollectionRoot<T> where T : IElement
    {
        [JsonProperty("elements", Required = Required.Always)]
        public List<T> Elements;
    }
}
