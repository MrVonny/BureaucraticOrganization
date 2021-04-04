

using Newtonsoft.Json;

namespace BureaucraticOrganization
{
    public class UnconditionalRule : Rule
    {
        [JsonProperty("event")]
        private RuleEvent _event;

        public UnconditionalRule(RuleEvent _event)
        {
            this._event = _event;
        }

        internal override void Execute(BypassSheet sheet)
        {
            sheet.PutStamp(_event.PutStampId);
            sheet.CrossStamp(_event.CrossStampId);
            sheet.SendToDepartment(_event.NextDepartmentId);
        }

    }
}
