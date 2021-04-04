using Newtonsoft.Json;

namespace BureaucraticOrganization
{
    public class RuleEvent
    {
        public RuleEvent(string putStampId, string crossStampId, string nextDepartmentId)
        {
            PutStampId = putStampId;
            CrossStampId = crossStampId;
            NextDepartmentId = nextDepartmentId;
        }

        [JsonProperty("putStampId")]
        public string PutStampId { get; }
        [JsonProperty("crossStampId")]
        public string CrossStampId { get; }
        [JsonProperty("nextDepartmentId")]
        public string NextDepartmentId { get; }
    }
}
