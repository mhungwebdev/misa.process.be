using MISA.PROCESS.COMMON.MISAAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.COMMON.Entities
{
    public class Department : InfoInteractive
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [PrimaryKey("Department")]
        [Length(36)]
        [Require]
        [FieldInsert]
        public Guid? DepartmentID { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [Require]
        [Length(255)]
        [FieldInsert]
        public String? DepartmentName { get; set; }
    }
}
