using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAccessManagement
{

    public enum PrivilegeType
    {
        prvCreate,
        prvRead, 
        prvWrite,
        prvDelete,
        prvAppend,
        prvAppendTo,
        prvAssign,
        prvShare
    }

    public class RolePrivileges
    {
        public string RoleName { get; set; }
        public Guid RoleId { get; set; }
        public Guid BusinessUnitId { get; set; }

        public Guid CreatePrivilegeId { get; set; }
        public int CreateDepth { get; set; }
        public Image CreateImag { get; set; }

        public Guid ReadPrivilegeId { get; set; }
        public int ReadDepth { get; set; }
        public Image ReadImag { get; set; }

        public Guid WritePrivilegeId { get; set; }
        public int WriteDepth { get; set; }
        public Image WriteImag { get; set; }

        public Guid DeletePrivilegeId { get; set; }
        public int DeleteDepth { get; set; }
        public Image DeleteImag { get; set; }

        public Guid AppendPrivilegeId { get; set; }
        public int AppendDepth { get; set; }
        public Image AppendImag { get; set; }

        public Guid AppendToPrivilegeId { get; set; }
        public int AppendToDepth { get; set; }
        public Image AppendToImag { get; set; }

        public Guid AssignPrivilegeId { get; set; }
        public int AssignDepth { get; set; }
        public Image AssignImag { get; set; }

        public Guid SharePrivilegeId { get; set; }
        public int ShareDepth { get; set; }
        public Image ShareImag { get; set; }
    }

    public class Privilege
    {
        public Guid PrivilegeId { get; set; }
        public PrivilegeType PrivilegeType { get; set; }
        public int Depth { get; set; }
        public Image Imag { get; set; }
    }

    public class ProcessList
    {
        public Guid RoleId { get; set; }
        public Guid BusinessUnitId { get; set; }
        public Guid PrivilegeId { get; set; }
        public int Depth { get; set; }
        public int Old_Depth { get; set; }
    }
}
