using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConfigManager.Entity
{
    public class Package : Command
    {
        [JsonProperty("dep", ItemIsReference = true, NullValueHandling = NullValueHandling.Include)]
        public List<string> Dependencies;
    }
}
