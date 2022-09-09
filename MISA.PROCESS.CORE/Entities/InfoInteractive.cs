using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.PROCESS.COMMON.MISAAttributes;

namespace MISA.PROCESS.COMMON
{
    public class InfoInteractive
    {
        /// <summary>
        /// Ngày tạo
        /// </summary>
        [FieldInsert]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        [FieldInsert]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [Length(100)]
        [FieldInsert]
        public String? CreatedBy { get; set; }

        /// <summary>
        /// Người sửa
        /// </summary>
        [FieldInsert]
        [Length(100)]
        public String? ModifiedBy { get; set; }
    }
}
