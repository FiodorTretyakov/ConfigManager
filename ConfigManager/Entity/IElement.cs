using Newtonsoft.Json;

namespace ConfigManager.Entity
{
    public interface IElement
    {

        [JsonProperty("name", Required = Required.Always)]
        string Name { get; set; }

        [JsonProperty("desc", NullValueHandling = NullValueHandling.Include)]
        string Description { get; set; }
}
}
