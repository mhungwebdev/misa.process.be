    using MISA.PROCESS.COMMON.MISAAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.COMMON.Entities
{
    public class Role
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [PrimaryKey("Role")]
        [Length(36)]
        [Require]
        [FieldInsert]
        public Guid RoleID { get; set; }

        /// <summary>
        /// Tên vai trò
        /// </summary>
        [Require]
        [Length(255)]
        [FieldInsert]

        public String? RoleName { get; set; }
    }
}
