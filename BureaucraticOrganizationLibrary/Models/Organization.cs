using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BureaucraticOrganization
{
    public class Organization
    {
        private OrganizationConfiguration configuration;

        public Organization(OrganizationConfiguration configuration)
        {
            Configure(configuration);
        }

        public Organization(string jsonConfiguration)
        {
            Configure(jsonConfiguration);
        }

        public void Configure(string jsonConfiguration)
        {
            JObject jConfiguration = JObject.Parse(jsonConfiguration);
            JArray jDepartments = (JArray)jConfiguration["departments"];
            List<Department> departments = new List<Department>();
            foreach (JObject jDep in jDepartments)
            {
                Department dep;
                JObject jRule = (JObject)jDep["rule"];
                if (jRule.Count == 1)
                {
                    JObject jEvent = (JObject)jRule["event"];
                    dep = new Department(
                        (string)jDep["id"],
                        JsonConvert.DeserializeObject<RuleEvent>(jEvent.ToString())
                        );
                }
                else if (jRule.Count == 3)
                {
                    string jconditionalStamp = (string)jRule["jconditionalStamp"];
                    JObject jEvent1 = (JObject)jRule["event1"];
                    JObject jEvent2 = (JObject)jRule["event2"];
                    dep = new Department(
                        (string)jDep["id"],
                        jconditionalStamp,
                        JsonConvert.DeserializeObject<RuleEvent>(jEvent1.ToString()),
                        JsonConvert.DeserializeObject<RuleEvent>(jEvent2.ToString()));
                }
                else
                {
                    throw new InvalidOperationException();
                }
                departments.Add(dep);
            }

            string startDeparment = (string)jConfiguration["startDepartment"];
            string endDepartment = (string)jConfiguration["endDepartment"];

            configuration = new OrganizationConfiguration(startDeparment, endDepartment, departments);
        }
        public void Configure(OrganizationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public BypassResult GetResult(string department)
        {
            return GetResult(department, TimeSpan.FromSeconds(1));
        }
        public BypassResult GetResult(string department, TimeSpan computationTimeLimit)
        {
            try
            {
                using (var tokenSource = new CancellationTokenSource(computationTimeLimit))
                {
                    var task = Task<BypassSheet>.Run(() => Execute(department, tokenSource.Token));
                    task.Wait();
                    return task.Result;
                }
            }
            catch (OperationCanceledException ex)
            {
                BypassResult result = new BypassResult();
                result.Exception = ex;
                return result;
            }
            catch (Exception ex)
            {
                BypassResult result = new BypassResult();
                result.Exception = ex;
                result.Successful = false;
                return result;
            }
        }
        public async Task<BypassResult> GetResultAsync(string department)
        {
            return await Task.Run(() => GetResult(department));
        }
        public async Task<BypassResult> GetResultAsync(string department, TimeSpan computationTimeLimit)
        {
            return await Task.Run(() => GetResult(department, computationTimeLimit));
        }


        private BypassResult Execute(string department, CancellationToken cts)
        {
            bool ShouldContuinue(BypassSheet _sheet)
            {
                lock(_sheet)          
                    lock (_sheet.Configuration)
                        return !_sheet.LastDepartment.Equals(configuration.EndDepartment);
                
            }

            BypassSheet sheet = new BypassSheet(configuration.StartDepartment, configuration);
            BypassResult result = new BypassResult();
            do
            {
                if (cts.IsCancellationRequested)
                {
                    result.Exception = new OperationCanceledException("Calculation time exceeded");
                    return result;
                }
                lock (sheet)
                {
                    sheet.CurrentDepartment.ExecuteRule(sheet);
                    if (sheet.LastDepartment.Id.Equals(department))
                        result.AddSnapshot(sheet.MakeSnaphot());
                }
            }
            while (ShouldContuinue(sheet));

            result.Successful = true;
            return result;
        }

    }
}
