using MISA.PROCESS.COMMON.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.COMMON.Entities
{
    public class RoleUpdate : Role
    {
        /// <summary>
        /// Trạng thái update
        /// </summary>
        public UpdateRoleMode UpdateMode { get; set; }
    }
}
