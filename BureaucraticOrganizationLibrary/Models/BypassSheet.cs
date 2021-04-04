using System.Collections.Generic;
using System.Linq;

namespace BureaucraticOrganization
{
    internal class BypassSheet
    {
        private Dictionary<string, StampState> stamps = new Dictionary<string, StampState>();


        public Department LastDepartment { get; private set; }
        public Department CurrentDepartment { get; private set; }
        public OrganizationConfiguration Configuration{ get; }

        public BypassSheet(Department currentDepartment, OrganizationConfiguration configuration)
        {
            CurrentDepartment = currentDepartment;
            Configuration = configuration;
        }


        internal void PutStamp(string stampId)
        {
            if (stamps.ContainsKey(stampId))
                stamps[stampId] = StampState.Putted;
            else
                stamps.Add(stampId, StampState.Putted);
        }

        internal void CrossStamp(string stampId)
        {
            if (stamps.ContainsKey(stampId))
                stamps[stampId] = StampState.Crossed;
        }

        internal bool HaveStamp(string stampId)
        {
            return stamps.ContainsKey(stampId) && stamps[stampId] == StampState.Putted;
        }

        internal void SendToDepartment(string id)
        {
            LastDepartment = CurrentDepartment;
            CurrentDepartment = Configuration.Departments.Single(d=>d.Id==id);
        }

        internal BypassSheetSnapshot MakeSnaphot()
        {
            return new BypassSheetSnapshot(stamps.Select(x => new Stamp(x.Key, x.Value)));
        }

    }
}
