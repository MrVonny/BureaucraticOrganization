using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BureaucraticOrganization
{
    public class Stamp
    {
        public Stamp(string id, StampState state)
        {
            Id = id;
            State = state;
        }
        [JsonProperty("id")]
        public string Id { get; }
        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StampState State { get; }
    }
}
