using Newtonsoft.Json;

namespace BureaucraticOrganization
{
    public class ConditionalRule : Rule
    {
        [JsonProperty]
        private string conditionalStamp;
        [JsonProperty]
        private RuleEvent event1;
        [JsonProperty]
        private RuleEvent event2;

        public ConditionalRule(string conditionalStamp, RuleEvent onConditionMet, RuleEvent onConditionNotMet)
        {
            this.conditionalStamp = conditionalStamp;
            event1 = onConditionMet;
            event2 = onConditionNotMet;
        }

        internal override void Execute(BypassSheet sheet)
        {
            if(sheet.HaveStamp(conditionalStamp))
            {
                sheet.PutStamp(event1.PutStampId);
                sheet.CrossStamp(event1.CrossStampId);
                sheet.SendToDepartment(event1.NextDepartmentId);
            }
            else
            {
                sheet.PutStamp(event2.PutStampId);
                sheet.CrossStamp(event2.CrossStampId);
                sheet.SendToDepartment(event2.NextDepartmentId);
            }
        }
    }
}
