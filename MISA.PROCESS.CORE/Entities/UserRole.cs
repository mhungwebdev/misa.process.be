using MISA.PROCESS.COMMON.MISAAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.COMMON.Entities
{
    public class UserRole : InfoInteractive
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [PrimaryKey("User_Role")]
        [Length(36)]
        public Guid UserID { get; set; }

        /// <summary>
        /// Khóa chính
        /// </summary>
        [PrimaryKey("User_Role")]
        [Length(36)]
        public Guid RoleID { get; set; }
    }
}
