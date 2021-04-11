using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BureaucraticOrganization
{
    public class OrganizationConfiguration
    {
        private string startDepartmentId;
        private string endDepartmentId;

        public OrganizationConfiguration(string startDepartmentId, string endDepartmentId, IReadOnlyList<Department> departments)
        {
            this.startDepartmentId = startDepartmentId;
            this.endDepartmentId = endDepartmentId;
            Departments = departments;
        }
        [JsonProperty("departments")]
        public IReadOnlyList<Department> Departments { get; }
        [JsonProperty("startDepartment")]
        public Department StartDepartment
        {
            get
            {
                return Departments.Single(d => d.Id == startDepartmentId);
            }
        }
        [JsonProperty("endDepartment")]
        public Department EndDepartment
        {
            get
            {
                return Departments.Single(d => d.Id == endDepartmentId);
            }
        }




    }
}
