using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BureaucraticOrganization
{
    public class BypassResult
    {
        private Exception exception;
        public List<BypassSheetSnapshot> BypassSheetSnapshots { get; internal set; } = new List<BypassSheetSnapshot>();

        public bool Successful { get; internal set; }
        public bool IsEmpty
        {
            get
            {
                return BypassSheetSnapshots.Count == 0;
            }
        }
        public Exception Exception { 
            get { 
                return exception;
            }
            internal set
            {
                exception = value;
                Successful = false;
            }
        }
        public bool IsLoop { get; internal set; }
        public string ToJson()
        {
            JObject result;
            if(!Successful)           
                result = new JObject(
                    new JProperty("successful", "false"),
                    new JProperty("exception", Exception.Message));          
            else
                result = new JObject(
                    new JProperty("successful", "true"),
                    !IsEmpty ?
                    new JProperty("snapshots", JArray.Parse(JsonConvert.SerializeObject(BypassSheetSnapshots))):
                    new JProperty("message", "The selected department has never been visited"));

            return result.ToString(Formatting.Indented);
        }

        internal void AddSnapshot(BypassSheetSnapshot snapshot)
        {
            BypassSheetSnapshots.Add(snapshot);
        }
    }

    public class BypassSheetSnapshot
    {
        public BypassSheetSnapshot(IEnumerable<Stamp> stamps)
        {
            Stamps = new List<Stamp>(stamps);
        }
        [JsonProperty("stamps")]
        public List<Stamp> Stamps { get; }
    }
}
