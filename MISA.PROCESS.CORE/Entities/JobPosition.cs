using MISA.PROCESS.COMMON.MISAAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.COMMON.Entities
{
    public class JobPosition : InfoInteractive
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [PrimaryKey("Job_Position")]
        [Length(36)]
        [Require]
        [FieldInsert]
        public Guid? JobPositionID { get; set; }

        /// <summary>
        /// Tên vị trí công việc
        /// </summary>
        [Require]
        [Length(255)]
        [FieldInsert]
        public String? JobPositionName { get; set; }
    }
}
