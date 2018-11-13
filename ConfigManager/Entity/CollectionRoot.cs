using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConfigManager.Entity
{
    public class CollectionRoot<T> where T: IElement
    {
        [JsonProperty("elements")]
        public List<T> Elements;
    }
}
