using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.COMMON.Enum;
using MISA.PROCESS.COMMON.MISAAttributes;
using MISA.PROCESS.DAL.Interfaces.InterfaceRepository;
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
        public UserRepository(IConfiguration configuration):base(configuration)
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

                var store = $"Proc_FilterUser";
                parameters.Add("Keyword",keyword);
                parameters.Add("RoleID",roleID);
                parameters.Add("LimitRecord",pageSize);
                parameters.Add("Offset",pageSize*(pageNumber - 1));
                parameters.Add("TotalRecord",direction:ParameterDirection.Output);

                var data = sqlConnection.Query<User>(store, parameters, commandType: CommandType.StoredProcedure);
                var totalRecord = parameters.Get<int>("TotalRecord");

                var rowEnd = pageSize*pageNumber;

                if(pageSize*pageNumber > totalRecord) {
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
                sqlConnection.Open();
                string newEmployeeCode = String.Empty;

                var sqlCommand = "Select EmployeeCode from User where EmployeeCode like '%NV-%' order by EmployeeCode desc limit 1 offset 0";
                string employeeCodeBiggest = sqlConnection.QuerySingleOrDefault<string>(sqlCommand);

                for (int i = employeeCodeBiggest.Length - 1; i > 0; i--)
                {
                    int n;
                    bool isNumeric = int.TryParse(employeeCodeBiggest[i].ToString(), out n);
                    if (isNumeric)
                    {
                        newEmployeeCode = employeeCodeBiggest[i].ToString() + newEmployeeCode;
                    }
                    else
                        break;
                }

                return "NV-" + (int.Parse(newEmployeeCode) + 1);
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
            using(sqlConnection = new MySqlConnection(connectString))
            {
                var storeGetUser = "Proc_GetUserById";
                var sqlCommandGetRoleIds = "Select * from Role Where RoleID in " +
                    "(Select RoleID From User_Role Where UserID = @UserID)";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id);

                User user = sqlConnection.QuerySingleOrDefault<User>(storeGetUser,parameters, commandType:CommandType.StoredProcedure);
                parameters.Add("@UserID",id);
                var roles = sqlConnection.Query<RoleUpdate>(sqlCommandGetRoleIds,parameters).ToList();

                user.Roles = roles;

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
        public override int InsertMulti(List<User> users)
        {
            sqlConnection = new MySqlConnection(connectString);
            sqlConnection.Open();
            using(var transaction = sqlConnection.BeginTransaction())
            {
                DynamicParameters parameters = new DynamicParameters();
                string sqlInsertMultiUser, sqlInsertMultiUserRole;
                HandleBuildCommandInsertMultiUser(users, parameters, out sqlInsertMultiUser, out sqlInsertMultiUserRole);

                parameters.Add("@Now", DateTime.Now);
                sqlInsertMultiUser = sqlInsertMultiUser[..^1];
                var userInseted = sqlConnection.Execute(sqlInsertMultiUser, transaction: transaction, param: parameters);
                sqlInsertMultiUserRole = sqlInsertMultiUserRole[..^1];
                sqlConnection.Execute(sqlInsertMultiUserRole, transaction: transaction, param: parameters);

                if (userInseted != users.Count)
                    transaction.Rollback();

                transaction.Commit();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return userInseted;
            }
        }
        #endregion

        #region Build Command Insert Multi User
        /// <summary>
        /// Build câu lệnh thêm mới user và userrole
        /// Author : mhungwebdev(3/9/2022)
        /// </summary>
        /// <param name="users">list user insert</param>
        /// <param name="parameters">param</param>
        /// <param name="sqlInsertMultiUser">câu sql insert nhiều user</param>
        /// <param name="sqlInsertMultiUserRole">câu sql insert nhiều user role</param>
        private static void HandleBuildCommandInsertMultiUser(List<User> users, DynamicParameters parameters, out string sqlInsertMultiUser, out string sqlInsertMultiUserRole)
        {
            sqlInsertMultiUser = $"INSERT INTO USER" +
                                $"(UserID, " +
                                $"EmployeeCode, " +
                                $"FullName, " +
                                $"DepartmentID, " +
                                $"JobPositionID, " +
                                $"Email, " +
                                $"ActiveStatus, " +
                                $"AvatarColor, " +
                                $"RoleNames, " +
                                $"CreatedBy, " +
                                $"CreatedDate, " +
                                $"ModifiedBy, " +
                                $"ModifiedDate) VALUES ";
            sqlInsertMultiUserRole = $"INSERT INTO User_Role " +
                                         $"(UserID," +
                                         $"RoleID," +
                                         $"CreatedBy," +
                                         $"CreatedDate," +
                                         $"ModifiedBy," +
                                         $"ModifiedDate) " +
                                         $"VALUES ";
            int userIndex = 0;
            int roleIndex = 0;
            foreach (User user in users)
            {
                string roleNames = string.Empty;
                sqlInsertMultiUser += $"(@UserID{userIndex}," +
                    $"@EmployeeCode{userIndex}," +
                    $"@FullName{userIndex}," +
                    $"@DepartmentID{userIndex}," +
                    $"@JobPositionID{userIndex}," +
                    $"@Email{userIndex}," +
                    $"@ActiveStatus{userIndex}," +
                    $"@AvatarColor{userIndex}," +
                    $"@RoleNames{userIndex}," +
                    $"'mhungwebdev'," +
                    $"@Now," +
                    $"'mhungwebdev'," +
                    $"@Now),";

                int count = 0;
                foreach (RoleUpdate roleUpdate in user.Roles)
                {
                    sqlInsertMultiUserRole += $"(@UserUserID{roleIndex}," +
                        $"@RoleRoleID{roleIndex}," +
                        $"'mhungwebdev'," +
                        $"@Now," +
                        $"'mhungwebdev'," +
                        $"@Now),";

                    parameters.Add($"@UserUserID{roleIndex}", user.UserID);
                    parameters.Add($"@RoleRoleID{roleIndex}", roleUpdate.RoleID);

                    if (count == 0)
                        roleNames += $"{roleUpdate.RoleName}";
                    else
                        roleNames += $"; {roleUpdate.RoleName}";

                    roleIndex++;
                    count++;
                }

                parameters.Add($"@UserID{userIndex}", user.UserID);
                parameters.Add($"@EmployeeCode{userIndex}", user.EmployeeCode);
                parameters.Add($"@FullName{userIndex}", user.FullName);
                parameters.Add($"@DepartmentID{userIndex}", user.DepartmentID);
                parameters.Add($"@JobPositionID{userIndex}", user.JobPositionID);
                parameters.Add($"@Email{userIndex}", user.Email);
                parameters.Add($"@ActiveStatus{userIndex}", user.ActiveStatus);
                parameters.Add($"@AvatarColor{userIndex}", user.AvatarColor);
                parameters.Add($"@RoleNames{userIndex}", roleNames);

                userIndex++;
            }
        }
        #endregion

        #region UpdateRole
        /// <summary>
        /// Update role cho user
        /// Author : mhungwebdev (30/8/2022)
        /// </summary>
        /// <param name="listLastUpdate">List role của user sau khi update</param>
        /// <param name="listDelete">List role sẽ xóa</param>
        /// <param name="listInsert">List role sẽ thêm mới</param>
        /// <param name="userID">Id của user sửa</param>
        /// <returns>Số bản ghi lưu thành công</returns>
        public int UpdateRole(List<RoleUpdate> listLastUpdate, List<RoleUpdate> listDelete, List<RoleUpdate> listInsert, Guid userID)
        {
            sqlConnection = new MySqlConnection(connectString);
            sqlConnection.Open();

            using(var transaction = sqlConnection.BeginTransaction())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserID", userID);
                parameters.Add("@Now", DateTime.Now);

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
                

                if (listDelete.Count > 0)
                {
                    int i = 0;
                    foreach(RoleUpdate roleUpdate in listDelete)
                    {
                        var sqlCommand = $"Delete from User_Role " +
                            $"where RoleID = @RoleID{i} And UserID = @UserID";
                        parameters.Add($"RoleID{i}",roleUpdate.RoleID);
                        sqlConnection.Execute(sqlCommand, param: parameters, transaction);
                        i++;
                    }
                }

                if (listInsert.Count > 0)
                {
                    var sqlInsertMultiUserRole = $"INSERT INTO User_Role " +
                             $"(UserID," +
                             $"RoleID," +
                             $"CreatedBy," +
                             $"CreatedDate," +
                             $"ModifiedBy," +
                             $"ModifiedDate) " +
                             $"VALUES ";
                    int rowIndex = 0;
                    foreach (RoleUpdate roleUpdate in listInsert)
                    {
                        sqlInsertMultiUserRole += $"(@UserID," +
                            $"@RoleID{rowIndex}," +
                            $"'mhungwebdev'," +
                            $"@Now," +
                            $"'mhungwebdev'," +
                            $"@Now),";

                        parameters.Add($"@UserID", userID);
                        parameters.Add($"@RoleID{rowIndex}", roleUpdate.RoleID);
                        rowIndex++;
                    }

                    sqlInsertMultiUserRole = sqlInsertMultiUserRole[..^1];
                    sqlConnection.Execute(sqlInsertMultiUserRole, param: parameters, transaction);
                }

                var sqlUpdateUser = $"Update User set " +
                    $"RoleNames = @RoleNames," +
                    $"ModifiedDate = @Now where UserID = @UserID";
                parameters.Add("@RoleNames",roleNames);
                var res = sqlConnection.Execute(sqlUpdateUser, param: parameters, transaction);
                transaction.Commit();
                sqlConnection.Dispose();
                sqlConnection.Close();

                return res;
            }
        }
        #endregion
    }
}
