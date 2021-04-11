using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BureaucraticOrganization
{
    public class Organization
    {
        public OrganizationConfiguration Configuration { get; private set; }

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
                    string jconditionalStamp = (string)jRule["conditionalStamp"];
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

            string startDeparment = (string)jConfiguration["startDepartment"] ?? throw new ArgumentNullException();
            string endDepartment = (string)jConfiguration["endDepartment"] ?? throw new ArgumentNullException();
            if (!departments.Any(dep => dep.Id.Equals(startDeparment)))
                throw new ArgumentException($"There is no department with this ID: {startDeparment}");
            if (!departments.Any(dep => dep.Id.Equals(endDepartment)))
                throw new ArgumentException($"There is no department with this ID: {endDepartment}");

            Configuration = new OrganizationConfiguration(startDeparment, endDepartment, departments);
        }
        public void Configure(OrganizationConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public BypassResult GetResult(string department)
        {
            try
            {
                var result = Execute(department);
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
        private BypassResult Execute(string department)
        {
            bool ShouldContuinue(BypassSheet _sheet, int[] _path, BypassResult _result)
            {
   
                        if (_sheet.LastDepartment.Equals(Configuration.EndDepartment))
                        {
                            return false;
                        }
                            
                if(IsLoop(_path))
                {
                    _result.IsLoop = true;
                    _result.BypassSheetSnapshots = _result.BypassSheetSnapshots.Distinct().ToList();
                    return false;
                }
                return true;
                
                
            }
            Dictionary<string, int> departmetsNumericId = new Dictionary<string, int>();
            string[] dep = Configuration.Departments.Select(x => x.Id).ToArray();

            for (int i = 0; i < dep.Length; i++)
                departmetsNumericId.Add(dep[i], i);

            List<int> path = new List<int>();

            BypassSheet sheet = new BypassSheet(Configuration.StartDepartment, Configuration);
            BypassResult result = new BypassResult();
            do
            {
                    sheet.CurrentDepartment.ExecuteRule(sheet);
                    if (sheet.LastDepartment.Id.Equals(department))
                        result.AddSnapshot(sheet.MakeSnaphot());
                    path.Add(departmetsNumericId[sheet.LastDepartment.Id]);
                
            }
            while (ShouldContuinue(sheet, path.ToArray(),result));

            result.Successful = true;
            return result;
        }
        private static bool IsLoop(int[] path)
        {
            int last = path.Length - 1;
            for (int substringLength = 1; substringLength <= path.Length / 2; substringLength++)
            {
                bool isLoop = true;
                for (int i = 0; i < substringLength; i++)
                {
                    if (path[last - i] != path[last - substringLength - i])
                        isLoop = false;
                }
                if (isLoop) return true;
                
            }
            return false;
        }

    }
}
