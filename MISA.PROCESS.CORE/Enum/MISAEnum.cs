using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.COMMON.Enum
{
    /// <summary>
    /// Trạng thái người dùng
    /// </summary>
    public enum ActiveStatus
    {
        /// <summary>
        /// Chưa kích hoạt
        /// </summary>
        NotActive = 0,

        /// <summary>
        /// Chờ kích hoạt
        /// </summary>
        WaitAcept = 1,

        /// <summary>
        /// Đang hoạt động
        /// </summary>
        Active = 2,

        /// <summary>
        /// Ngừng kích hoạt
        /// </summary>
        StopActive = 3
    }

    /// <summary>
    /// Trạng thái update role
    /// </summary>
    public enum UpdateRoleMode
    {
        /// <summary>
        /// Xóa role
        /// </summary>
        Delete = 1,

        /// <summary>
        /// Thêm mới role
        /// </summary>
        Insert = 2,

        /// <summary>
        /// Không làm gì cả
        /// </summary>
        NotDoEverything = 0
    }

    /// <summary>
    /// Trạng thái tương tác với DB
    /// </summary>
    public enum InteractiveMode
    {
        /// <summary>
        /// Tương tác nhiểu bản ghi
        /// </summary>
        Multi = 1,

        /// <summary>
        /// Tương tác một bản ghi
        /// </summary>
        Single = 2,
    }
}
