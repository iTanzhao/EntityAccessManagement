using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAccessManagement
{
    class CRMHelper
    {
        public static EntityCollection GetAllRoles(IOrganizationService Service)
        {
            QueryExpression Query = new QueryExpression("role")
            {
                Distinct = false,
                NoLock = true,
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression()
            };
            Query.Criteria.AddCondition("parentroleid", ConditionOperator.Null);
            return Service.RetrieveMultiple(Query);

        }
        public static Entity GetPrivilege(IOrganizationService Service,string name)
        {
            QueryExpression Query = new QueryExpression("privilege")
            {
                Distinct = false,
                NoLock = true,
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression()
            };
            Query.Criteria.AddCondition("name", ConditionOperator.Equal, name);
            EntityCollection Collection = Service.RetrieveMultiple(Query);
            if (Collection.Entities != null && Collection.Entities.Count > 0)
            {
                return Collection.Entities[0];
            }
            else
            {
                return null;
            }
        }

        public static EntityCollection GetRoleprivileges(IOrganizationService Service, string privilegeid)
        {
            QueryExpression Query = new QueryExpression("roleprivileges")
            {
                Distinct = false,
                NoLock = true,
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression()
            };
            Query.Criteria.AddCondition("privilegeid", ConditionOperator.Equal, privilegeid);
            return Service.RetrieveMultiple(Query);
        }

        public static void UpdatePrivilegesRole(IOrganizationService Service,ProcessList processList)
        {
            if (processList.Depth == 0&& processList.Old_Depth!=0)
            {
                RemovePrivilegeRoleRequest roleRequest = new RemovePrivilegeRoleRequest
                {
                    RoleId = processList.RoleId,
                    PrivilegeId = processList.PrivilegeId
                };
                Service.Execute(roleRequest);
            }
            else
            {
                AddPrivilegesRoleRequest request = new AddPrivilegesRoleRequest
                {
                    RoleId = processList.RoleId
                };
                RolePrivilege rp = new RolePrivilege();

                //Basic: 1/4,Local : 1/2,Deep : 3/4,Global: 4/4
                if (processList.Depth == 1) rp.Depth = PrivilegeDepth.Basic;
                else if (processList.Depth == 2) rp.Depth = PrivilegeDepth.Local;
                else if (processList.Depth == 4) rp.Depth = PrivilegeDepth.Deep;
                else if (processList.Depth == 8) rp.Depth = PrivilegeDepth.Global;

                rp.PrivilegeId = processList.PrivilegeId;
                rp.BusinessUnitId = processList.BusinessUnitId;
                request.Privileges = new RolePrivilege[] { rp };
                Service.Execute(request);
            }
        }
    }
}
