using Newtonsoft.Json;

namespace BureaucraticOrganization
{
    public class Department
    {
        [JsonProperty("rule")]
        internal Rule Rule { get; }
        [JsonProperty("id")]
        public string Id { get; }

        public Department(string id, RuleEvent ruleConfiguration)
        {
            Id = id;
            this.Rule = new UnconditionalRule(ruleConfiguration);
        }
        public Department(string id, string conditionalStamp, RuleEvent onConditionMet, RuleEvent onConditionNotMet)
        {
            Id = id;
            this.Rule = new ConditionalRule(conditionalStamp, onConditionMet, onConditionNotMet);
        }

        internal void ExecuteRule(BypassSheet sheet)
        {
            Rule.Execute(sheet);
        }

    }
}
