using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.COMMON.Enum;
using MISA.PROCESS.COMMON.MISAAttributes;
using MISA.PROCESS.DAL.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.DAL.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        #region Contructor
        public UserRepository(IConfiguration configuration) : base(configuration)
        {

        }
        #endregion

        #region Filter
        /// <summary>
        /// Phân trang + tìm kiếm
        /// Author : mhungwebdev (28/8/2022)
        /// </summary>
        /// <param name="pageSize">Số bản ghi trên một trang</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <param name="roleID">Id vai trò</param>
        /// <returns>Total record, RowStart, RowEnd,CurrentPage, Data (ds user)</returns>
        public object Filter(int pageSize, int pageNumber, string? keyword, Guid? roleID)
        {
            using (sqlConnection = new MySqlConnection(connectString))
            {
                DynamicParameters parameters = new DynamicParameters();

                var store = $"Proc_User_Filter";
                parameters.Add("Keyword", keyword);
                parameters.Add("RoleID", roleID);
                parameters.Add("LimitRecord", pageSize);
                parameters.Add("Offset", pageSize * (pageNumber - 1));
                parameters.Add("TotalRecord", direction: ParameterDirection.Output);

                var data = sqlConnection.Query<User>(store, parameters, commandType: CommandType.StoredProcedure);
                var totalRecord = parameters.Get<int>("TotalRecord");

                var rowEnd = pageSize * pageNumber;

                if (pageSize * pageNumber > totalRecord)
                {
                    rowEnd = totalRecord;
                }

                var res = new
                {
                    TotalRecord = totalRecord,
                    RowStart = pageSize * (pageNumber - 1) + 1,
                    RowEnd = rowEnd,
                    CurrentPage = pageNumber,
                    Data = data
                };

                return res;
            }
        }
        #endregion

        #region GenerateNewEmployeeCode
        /// <summary>
        /// Sinh mã nhân viên mới
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <returns>Mã nhân viên mới</returns>
        public string GenerateNewEmployeeCode()
        {
            using (sqlConnection = new MySqlConnection(connectString))
            {
                var sqlGetNewCode = "Proc_User_GetNewEmployeeCode";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NewEmployeeCode", direction: ParameterDirection.Output);

                sqlConnection.Execute(sqlGetNewCode, parameters, commandType: CommandType.StoredProcedure);
                var newEmployeeCode = parameters.Get<string>("NewEmployeeCode");

                return newEmployeeCode;
            }
        }
        #endregion

        #region Get User by id
        /// <summary>
        /// Lấy User theo id
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override User Get(Guid id)
        {
            using (sqlConnection = new MySqlConnection(connectString))
            {
                var storeGetUser = "Proc_User_GetById";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id);

                var multiples = sqlConnection.QueryMultiple(storeGetUser, parameters, commandType: CommandType.StoredProcedure);

                User user = multiples.Read<User>().First();
                user.Roles = multiples.Read<RoleUpdate>().ToList();

                return user;
            }
        }
        #endregion

        #region Insert Multi
        /// <summary>
        /// Thêm mới hàng loạt
        /// Author : mhungwebdev (30/8/2022)
        /// </summary>
        /// <param name="users">List user thêm mới</param>
        /// <returns>Số bản ghi thêm mới thành công</returns>
        public int InsertMultiUserAndUserRole(List<User> users, IDbTransaction transaction)
        {
            List<UserRole> userRoles = HandleUserRoleInsert(users);

            var userInserted = InsertMulti<User>(users, transaction);
            var userRoleInserted = InsertMulti<UserRole>(userRoles, transaction);

            if (userInserted != users.Count || userRoleInserted != userRoles.Count)
                transaction.Rollback();

            return userInserted;
        }
        #endregion

        #region HandleUserRoleInsert
        /// <summary>
        /// Build list user role và xử lý RoleNames cho user
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="users">list user insert hàng loạt</param>
        /// <returns>list user role</returns>
        List<UserRole> HandleUserRoleInsert(List<User> users)
        {
            List<UserRole> userRoles = new List<UserRole>();

            for (int i=0; i < users.Count; i++)
            {
                User user = users[i] as User;
                foreach (RoleUpdate role in user.Roles)
                    userRoles.Add(new UserRole(user.UserID, role.RoleID));
                user.RoleNames = HandleGetRoleNames(user.Roles);
            }

            return userRoles;
        }
        #endregion

        #region UpdateRole
        /// <summary>
        /// Update role cho user
        /// Author : mhungwebdev (30/8/2022)
        /// </summary>
        /// <param name="listLastUpdate">List role của user sau khi update</param>
        /// <param name="listDelete">List id user role sẽ xóa</param>
        /// <param name="listInsert">List user role sẽ thêm mới</param>
        /// <param name="userID">Id của user sửa</param>
        /// <returns>Số bản ghi lưu thành công</returns>
        public int UpdateRole(List<RoleUpdate> listLastUpdate, List<Guid> listDelete, List<UserRole> listInsert, Guid userID, IDbTransaction transaction)
        {

            DynamicParameters parameters = new DynamicParameters();

            int userRoleDeleted, userRoleInserted;
            HandleUpdateUserRole(listDelete, listInsert, userID, transaction, parameters, out userRoleDeleted, out userRoleInserted);

            string roleNames = HandleGetRoleNames(listLastUpdate);
            var sqlUpdateUser = "Proc_User_UpdateRoleNames";
            parameters.Add("RoleNames", roleNames);
            parameters.Add("UserID", userID);

            var res = sqlConnection.Execute(sqlUpdateUser, param: parameters, transaction,commandType:CommandType.StoredProcedure);

            if (res != 1 || userRoleDeleted != listDelete.Count || userRoleInserted != listInsert.Count)
                transaction.Rollback();

            return res;
        }
        #endregion

        #region HandleUpdateUserRole
        /// <summary>
        /// Cập nhật user role cho user
        /// Author : mhungwebdev (10/9/2022)
        /// </summary>
        /// <param name="listDelete">list id user role xóa</param>
        /// <param name="listInsert">list user role thêm mới</param>
        /// <param name="userID">id của user</param>
        /// <param name="transaction">transaction</param>
        /// <param name="parameters">parameter</param>
        /// <param name="userRoleDeleted">Số user role xóa được</param>
        /// <param name="userRoleInserted">Số user role thêm mới được</param>
        private void HandleUpdateUserRole(List<Guid> listDelete, List<UserRole> listInsert, Guid userID, IDbTransaction transaction, DynamicParameters parameters, out int userRoleDeleted, out int userRoleInserted)
        {
            userRoleDeleted = 0;
            userRoleInserted = 0;

            if (listDelete.Count > 0)
                userRoleDeleted = DeleteMulti<UserRole>(listDelete, transaction, "UserID", userID);

            if (listInsert.Count > 0)
                userRoleInserted = InsertMulti<UserRole>(listInsert, transaction);

        }
        #endregion

        #region HandleGetRoleNames
        /// <summary>
        /// Xử lý lấy rolenames cho user
        /// Author : mhungwebdev (10/9/2022)
        /// </summary>
        /// <param name="listLastUpdate">list role update của user</param>
        /// <returns>rolenames mới</returns>
        private static string HandleGetRoleNames(List<RoleUpdate> listLastUpdate)
        {
            string roleNames = string.Empty;
            int roleNameIndex = 0;

            foreach (RoleUpdate roleUpdate in listLastUpdate)
            {
                if (roleNameIndex == 0)
                    roleNames += $"{roleUpdate.RoleName}";
                else
                    roleNames += $"; {roleUpdate.RoleName}";
                roleNameIndex++;
            }

            return roleNames;
        }
        #endregion
    }
}
