using MISA.PROCESS.COMMON.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.BLL.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        /// <summary>
        /// Thêm mới hàng loạt
        /// Author : mhungwebdev (30/8/2022)
        /// </summary>
        /// <param name="users">List user thêm mới</param>
        /// <returns>Số bản ghi thêm mới thành công</returns>
        int InsertMulti(List<User> users);

        /// <summary>
        /// Sửa User
        /// Author : mhungwebdev (30/8/2022)
        /// </summary>
        /// <param name="roles">List role update</param>
        /// <returns>Số bản ghi sửa thành công</returns>
        int UpdateRole(List<RoleUpdate> roles, Guid userID);
    }
}
