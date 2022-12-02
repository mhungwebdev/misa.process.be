using MISA.PROCESS.BLL.Interfaces;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.COMMON.Enum;
using MISA.PROCESS.COMMON.Resources;
using MISA.PROCESS.DAL.Interfaces;
using MISA.PROCESS.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MISA.PROCESS.BLL.Services
{
    public class UserService : BaseService<User>,IUserService
    {
        #region Contructor
        IUserRepository _userRepository;
        public UserService(IUserRepository userRepository):base(userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region UpdateRole
        /// <summary>
        /// Sửa User
        /// Author : mhungwebdev (30/8/2022)
        /// </summary>
        /// <param name="roles">List role update</param>
        /// <returns>Số bản ghi sửa thành công</returns>
        public int UpdateRole(List<RoleUpdate> roles, Guid userID)
        {
            List<Guid> listDelete;
            List<UserRole> listInsert;
            List<RoleUpdate> listLastUpdate;
            HandleListRoleUpdate(roles, out listDelete, out listInsert, out listLastUpdate,userID);

            connection = _userRepository.GetConnection();
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var res = _userRepository.UpdateRole(listLastUpdate, listDelete, listInsert, userID,transaction);

                if (res != 1)
                    transaction.Rollback();
                else
                    transaction.Commit();
;
                connection.Dispose();
                connection.Close();
                return res;
            }

        }
        #endregion

        #region Xử lý mảng role update
        /// <summary>
        /// Xử lý mảng role update
        /// Author : mhungwebdev (5/9/2022)
        /// </summary>
        /// <param name="roles">mảng role update</param>
        /// <param name="listDelete">mảng id user role sẽ xóa</param>
        /// <param name="listInsert">mảng user role sẽ insert</param>
        /// <param name="listLastUpdate">mảng role cuối cùng của user</param>
        /// <param name="userID">Id đối tượng muốn sửa role</param>
        private static void HandleListRoleUpdate(List<RoleUpdate> roles, out List<Guid> listDelete, out List<UserRole> listInsert, out List<RoleUpdate> listLastUpdate,Guid userID)
        {
            listDelete = new List<Guid>();
            listInsert = new List<UserRole>();
            listLastUpdate = new List<RoleUpdate>();
            foreach (RoleUpdate roleUpdate in roles)
            {
                switch (roleUpdate.UpdateMode)
                {
                    case UpdateRoleMode.Delete:
                        listDelete.Add(roleUpdate.RoleID);
                        break;
                    case UpdateRoleMode.Insert:
                        listLastUpdate.Add(roleUpdate);
                        listInsert.Add(new UserRole(userID, roleUpdate.RoleID));
                        break;
                    case UpdateRoleMode.NotDoEverything:
                        listLastUpdate.Add(roleUpdate);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Validate Custom
        public override Dictionary<string, string> ValidateCustom(User user, Guid? id = null)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            var employeeCodeEnd = user.EmployeeCode[user.EmployeeCode.Length - 1];

            if (!(int.TryParse(employeeCodeEnd.ToString(), out int n)))
            {
                errors.Add("EmployeeCode", Resources.EmployeeCodeErrorMsg);
            }

            return errors;
        }
        #endregion

        #region DoInsertMulti
        /// <summary>
        /// Thêm mới hàng loạt
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="users">list user thêm mới</param>
        /// <returns>số bản ghi thêm mới thành công</returns>
        public override int DoInsertMulti(List<User> users, IDbTransaction transaction)
        {
            return _userRepository.InsertMultiUserAndUserRole(users, transaction);
        }
        #endregion
    }
}
