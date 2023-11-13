using Newtonsoft.Json;

namespace CommandForge.Models
{
    public class ConfigFile
    {
        [JsonProperty(Required = Required.Always)]
        public Default Defaults { get; set; }

        public struct Default
        {
            [JsonProperty(Required = Required.Always)]
            public bool EnableLogging { get; set; }

            [JsonProperty(Required = Required.Always)]
            public string ZmqSubscriberIPv4 { get; set; }

            [JsonProperty(Required = Required.Always)]
            public int ZmqSubscriberPort { get; set; }

            [JsonProperty(Required = Required.Always)]
            public string ZmqPublisherIPv4 { get; set; }

            [JsonProperty(Required = Required.Always)]
            public int ZmqPublisherPort { get; set; }
        }
    }
}