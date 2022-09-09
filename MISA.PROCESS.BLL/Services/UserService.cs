using MISA.core.Exceptions;
using MISA.PROCESS.BLL.Interfaces;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.COMMON.Enum;
using MISA.PROCESS.COMMON.Resources;
using MISA.PROCESS.DAL.Interfaces;
using System;
using System.Collections.Generic;
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
            List<RoleUpdate> listDelete, listInsert, listLastUpdate;
            HandleListRoleUpdate(roles, out listDelete, out listInsert, out listLastUpdate);

            var res = _userRepository.UpdateRole(listLastUpdate, listDelete, listInsert, userID);

            return res;
        }
        #endregion

        #region Xử lý mảng role update
        /// <summary>
        /// Xử lý mảng role update
        /// Author : mhungwebdev (5/9/2022)
        /// </summary>
        /// <param name="roles">mảng role update</param>
        /// <param name="listDelete">mảng role sẽ xóa</param>
        /// <param name="listInsert">mảng role sẽ insert</param>
        /// <param name="listLastUpdate">mảng role cuối cùng của user</param>
        private static void HandleListRoleUpdate(List<RoleUpdate> roles, out List<RoleUpdate> listDelete, out List<RoleUpdate> listInsert, out List<RoleUpdate> listLastUpdate)
        {
            listDelete = new List<RoleUpdate>();
            listInsert = new List<RoleUpdate>();
            listLastUpdate = new List<RoleUpdate>();
            foreach (RoleUpdate roleUpdate in roles)
            {
                if (roleUpdate.UpdateMode != UpdateRoleMode.Delete)
                {
                    listLastUpdate.Add(roleUpdate);
                    if (roleUpdate.UpdateMode == UpdateRoleMode.Insert)
                        listInsert.Add(roleUpdate);
                }
                else
                {
                    listDelete.Add(roleUpdate);
                }
            }
        }
        #endregion

        #region Validate Custom
        public override Dictionary<string, string> ValidateCustom(User user, Guid? id)
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
    }
}
