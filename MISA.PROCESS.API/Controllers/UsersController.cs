using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PROCESS.BLL.Interfaces.InterfaceService;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.DAL.Interfaces.InterfaceRepository;

namespace MISA.PROCESS.API.Controllers
{
    public class UsersController : MISABaseController<User>
    {
        IUserRepository _userRepository;
        IUserService _userService;

        public UsersController(IUserRepository userRepository, IUserService userService) : base(userRepository, userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        }

        #region Filter
        /// <summary>
        /// Api phân trang + tìm kiếm
        /// Author : mhungwebdev (28/8/2022)
        /// </summary>
        /// <param name="pageSize">Số bản ghi trên một trang</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <param name="roleID">Id vai trò</param>
        /// <returns>Total record, RowStart, RowEnd,CurrentPage, Data (ds user)</returns>
        [HttpGet("filter")]
        public IActionResult Filter(int pageSize, int pageNumber, string? keyword, Guid? roleID)
        {
            var res = _userRepository.Filter(pageSize, pageNumber, keyword, roleID);

            return Ok(res);
        }
        #endregion

        #region GererateNewEmployeeCode
        /// <summary>
        /// Sinh mã nhân viên mới
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <returns></returns>
        [HttpGet("NewEmployeeCode")]
        public IActionResult GererateNewEmployeeCode()
        {
            var res = _userRepository.GenerateNewEmployeeCode();

            return Ok(res);
        }
        #endregion

        #region UpdateRole
        /// <summary>
        /// Update role cho user
        /// Author : mhungwebdev (30/8/2022)
        /// </summary>
        /// <param name="roles">List roles update</param>
        /// <param name="id">id của user muốn update</param>
        /// <returns></returns>
        [HttpPut("UpdateRole/{id}")]
        public IActionResult UpdateRole(List<RoleUpdate> roles, Guid id)
        {
            var res = _userService.UpdateRole(roles,id);

            return Ok(res);
        }
        #endregion
    }
}
