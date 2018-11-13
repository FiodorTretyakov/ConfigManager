using Newtonsoft.Json;

namespace ConfigManager.Entity
{
    public class Command
    {

        [JsonProperty("name", Required = Required.Always)]
        public string Name;

        [JsonProperty("desc")]
        public string Description;
    }
}
