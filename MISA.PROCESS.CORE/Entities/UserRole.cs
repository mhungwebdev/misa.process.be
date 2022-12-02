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
        [FieldInsert]
        public Guid? RoleID { get; set; }

        /// <summary>
        /// Khóa chính
        /// </summary>
        [PrimaryKey("User_Role")]
        [Length(36)]
        [FieldInsert]
        public Guid? UserID { get; set; }

        public UserRole(Guid? userID, Guid? roleID)
        {
            UserID = userID;
            RoleID = roleID;
        }
    }
}
