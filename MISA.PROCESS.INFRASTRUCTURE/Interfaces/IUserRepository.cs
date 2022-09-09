using MISA.PROCESS.COMMON.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.DAL.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Phân trang + tìm kiếm
        /// Author : mhungwebdev (28/8/2022)
        /// </summary>
        /// <param name="pageSize">Số bản ghi trên một trang</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <param name="roleID">Id vai trò</param>
        /// <returns>Total record, RowStart, RowEnd,CurrentPage, Data (ds user)</returns>
        object Filter(int pageSize, int pageNumber, string? keyword, Guid? roleID);

        /// <summary>
        /// Sinh mã nhân viên mới
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <returns>Mã nhân viên mới</returns>
        string GenerateNewEmployeeCode();

        /// <summary>
        /// Update role cho user
        /// Author : mhungwebdev (30/8/2022)
        /// </summary>
        /// <param name="listLastUpdate">List role của user sau khi update</param>
        /// <param name="listDelete">List role sẽ xóa</param>
        /// <param name="listInsert">List role sẽ thêm mới</param>
        /// <param name="userID">Id của user sửa</param>
        /// <returns>Số bản ghi lưu thành công</returns>
        int UpdateRole(List<RoleUpdate> listLastUpdate,List<RoleUpdate> listDelete,List<RoleUpdate> listInsert,Guid userID);
    }
}
