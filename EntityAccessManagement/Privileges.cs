using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAccessManagement
{
    public class EntityPrivileges
    {

        private const string base64_0 = "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAADDSURBVChTlZIxEoIwEEV/Yst4B+20lY6KUjyOVNrQaMNVtM0NGBpqSqXlFGt+EhwKBoY3E2bZ/fzdBSBjylIkTUVONs3DmLkRGqSqgOwCdB2Q54Dp/WHMHGvUEPfYORN5vV04CWvUWDZFFBVQyrpdvcMUxwNQ10DbQsMY2zILlRmoodYt1/eu3SzUWK1fegUa2xRomnA7AzVWq/+zLTHs6uZb8VoVL+6jPJ7Afudd4ti7cgw6f77A/QYkSegwsPhriPwARl/+WRAN1PwAAAAASUVORK5CYII=";

        private const string base64_1 = "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAADWSURBVChTlZI9DwFBEIbfXRWNKETro1Nfp1Ke36LUqVWUfob6lCqVU0l0jk5QiIaQyJjZOUJyRzzJJjs7z8xsbs8QgyfrAXAIgMxE43sTKLaASkdjRguOU2DVA3JVFfKeZk+hNjhHQK0LFBqAFNDMJ9qO3DYRyYnDGIr6hMsGqA+1axrLNpAtw7qRco1fiCMuhXyr686N+4o47Nq4/pO50ZWAdZ9uUYpD5l1834vDrn3d/7ZP7ipnkhPY1XcIuciM9TAN8gEv4AmCPIocpCE5cZg/fw3gAcjnoTc0cueXAAAAAElFTkSuQmCC";

        private const string base64_2 = "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAADBSURBVChTlZItEwFRFIbfuxLFCLqPJm+TxPVbRE2WiH6GvKIk2axZmjGCURjBHOe9Z7dZ1hN27r3nOR+z9zpRkHOYA5cYqKxt/xoAzSHQHttesYTrBthPgVrHhHpo0VtiBe4p0J0AjT7ABNlGIqelX36EMTqKk3QmeByB3sKqFrEbAdUWAt+SY/yCDl1JdKrn2bf7Ch11HT9ZjVIE/teVRd2g1Pw56to9JJrkVtlpARIBYawdCC+FB0UwRkf582kAb61/mZD6roCtAAAAAElFTkSuQmCC";

        private const string base64_4 = "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAADASURBVChThZLBDYJAEEVn9wBNaGIHdiF7W4qwB8rgrLGI5SLBAijABjSGg6EBAwfH/HHXEALykk1m/rwBsoF4QH7N2ZwN04HkoEY2RBbqZ822siKVj5LbVysHNTLM4PwWELibk2AKzOAAwivDJ8wR5nB11VQUUEdF/bv33Zf4FPuKCK6S7RHdvqNIRyKPHzC58A9t1saXy8DVySrx7TJwFW4hvaRU3AsfT2M3ltzOkUaTbTMJ5sAMjiAX7Vn+NZg/2cPhmp1SOvcAAAAASUVORK5CYII=";

        private const string base64_8 = "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAACjSURBVChTY/iPBPov9f/32Obxn2EmAxiD2CAxZMAIIo6/PM7QebGTYeODjQzYgL+CP0O5fjmDpbglA1hDwK4AnIphAKRpg9sGBmaBYIGGiVcmQoVxg5sfbjIIsAkwMDP6Mjbc+XQHKowfMDIyMjCCPUgCYILSRAMmD1kPKJMwAKllcpdxh3IJA5BakoMV7AdQpIAEcAFYxIEByAYYIJw0/v8HAF2lfTA6QpTkAAAAAElFTkSuQmCC";
        public string EntityName { get; set; }
        public List<RolePrivileges> RolePrivileges { get; set; }

        public EntityPrivileges()
        {

        }
        public void SetPrivilegeId(PrivilegeType privilegeType, Entity privileges)
        {
            if (privileges != null)
            {
                switch (privilegeType)
                {
                    case PrivilegeType.prvCreate:
                        {
                            foreach (var rolePrivileges in RolePrivileges)
                                rolePrivileges.CreatePrivilegeId = privileges.Id;
                        }; break;
                    case PrivilegeType.prvRead:
                        {
                            foreach (var rolePrivileges in RolePrivileges)
                                rolePrivileges.ReadPrivilegeId = privileges.Id;
                        }; break;
                    case PrivilegeType.prvWrite:
                        {
                            foreach (var rolePrivileges in RolePrivileges)
                                rolePrivileges.WritePrivilegeId = privileges.Id;
                        }; break;
                    case PrivilegeType.prvDelete:
                        {
                            foreach (var rolePrivileges in RolePrivileges)
                                rolePrivileges.DeletePrivilegeId = privileges.Id;
                        }; break;
                    case PrivilegeType.prvAppend:
                        {
                            foreach (var rolePrivileges in RolePrivileges)
                                rolePrivileges.AppendPrivilegeId = privileges.Id;
                        }; break;
                    case PrivilegeType.prvAppendTo:
                        {
                            foreach (var rolePrivileges in RolePrivileges)
                                rolePrivileges.AppendToPrivilegeId = privileges.Id;
                        }; break;
                    case PrivilegeType.prvAssign:
                        {
                            foreach (var rolePrivileges in RolePrivileges)
                                rolePrivileges.AssignPrivilegeId = privileges.Id;
                        }; break;
                    case PrivilegeType.prvShare:
                        {
                            foreach (var rolePrivileges in RolePrivileges)
                                rolePrivileges.SharePrivilegeId = privileges.Id;
                        }; break;
                    default:
                        break;
                }
            }
        }
            
        public void SetRolePrivileges(PrivilegeType privilegeType, Entity roleprivileges)
        {
            RolePrivileges rolePrivileges = RolePrivileges.Where(t => t.RoleId.ToString() == roleprivileges["roleid"].ToString()).FirstOrDefault();
            if (rolePrivileges != null)
            {
                int Depth = (int)roleprivileges["privilegedepthmask"];
                Image img = GetPrivilegeLevel(Depth);
                switch (privilegeType)
                {
                    case PrivilegeType.prvCreate:
                        {
                            rolePrivileges.CreateDepth = Depth;
                            rolePrivileges.CreateImag = img;
                        }; break;
                    case PrivilegeType.prvRead:
                        {
                            rolePrivileges.ReadDepth = Depth;
                            rolePrivileges.ReadImag = img;
                        }; break;
                    case PrivilegeType.prvWrite:
                        {
                            rolePrivileges.WriteDepth = Depth;
                            rolePrivileges.WriteImag = img;
                        }; break;
                    case PrivilegeType.prvDelete:
                        {
                            rolePrivileges.DeleteDepth = Depth;
                            rolePrivileges.DeleteImag = img;
                        }; break;
                    case PrivilegeType.prvAppend:
                        {
                            rolePrivileges.AppendDepth = Depth;
                            rolePrivileges.AppendImag = img;
                        }; break;
                    case PrivilegeType.prvAppendTo:
                        {
                            rolePrivileges.AppendToDepth = Depth;
                            rolePrivileges.AppendToImag = img;
                        }; break;
                    case PrivilegeType.prvAssign:
                        {
                            rolePrivileges.AssignDepth = Depth;
                            rolePrivileges.AssignImag = img;
                        }; break;
                    case PrivilegeType.prvShare:
                        {
                            rolePrivileges.ShareDepth = Depth;
                            rolePrivileges.ShareImag = img;
                        }; break;
                    default:
                        break;
                }
            }
        }

        public Image GetPrivilegeLevel(int Depth)
        {
            string base64;
            if (Depth == 1) base64 = base64_1;
            else if (Depth == 2) base64 = base64_2;
            else if (Depth == 4) base64 = base64_4;
            else if (Depth == 8) base64 = base64_8;
            else base64 = base64_0;

            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            Image result = Image.FromStream(memStream);
            result.Tag = Depth;
            memStream.Close();
            return result;
        }

    }
}
