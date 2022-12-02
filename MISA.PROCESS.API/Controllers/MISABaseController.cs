using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PROCESS.BLL.Interfaces;
using MISA.PROCESS.DAL.Interfaces;

namespace MISA.PROCESS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MISABaseController<MISAEntity> : ControllerBase
    {
        #region Contructor
        IBaseRepository<MISAEntity> _baseRepository;
        IBaseService<MISAEntity> _baseService;

        public MISABaseController(IBaseRepository<MISAEntity> baseRepository, IBaseService<MISAEntity> baseService)
        {
            _baseRepository = baseRepository;
            _baseService = baseService;
        }
        #endregion

        #region Get
        /// <summary>
        /// Lấy tất cả bản ghi
        /// Author : mhungwebdev (28/8/2022)
        /// </summary>
        /// <returns>Tất cả bản ghi</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var res = _baseRepository.Get();

            return Ok(res);
        }
        #endregion

        #region Get by id
        /// <summary>
        /// Lấy theo id
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="id">id của bản ghi</param>
        /// <returns>Bản ghi có id tương ứng</returns>
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var res = _baseRepository.Get(id);

            return Ok(res);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Xóa theo id
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="id">id bản ghi muốn xóa</param>
        /// <returns>1 nếu thành công</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var res = _baseRepository.Delete(id);

            return Ok(res);
        }
        #endregion

        #region Insert
        /// <summary>
        /// Thêm mới 1 bản ghi
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">bản ghi mới</param>
        /// <returns>1 nếu thành công</returns>
        [HttpPost]
        public IActionResult Insert(MISAEntity entity)
        {
            var res = _baseService.Insert(entity);

            return StatusCode(201, res);
        }
        #endregion

        #region InsertMulti
        /// <summary>
        /// Thêm mới hàng loạt
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="entities">list record thêm mới</param>
        /// <returns>Số bản ghi thêm mới thành công</returns>
        [HttpPost("InsertMulti")]
        public IActionResult InsertMulti(List<MISAEntity> entities)
        {
            var res = _baseService.InsertMulti(entities);

            return StatusCode(201, res);
        }
        #endregion

        #region Update
        /// <summary>
        /// Sửa bản ghi
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">bản ghi cập nhật</param>
        /// <param name="id">id bản ghi cập nhật</param>
        /// <returns>1 nếu update thành công</returns>
        [HttpPut("{id}")]
        public virtual IActionResult Update(MISAEntity entity, Guid id)
        {
            var res = _baseService.Update(entity, id);

            return StatusCode(200, res);
        }
        #endregion
    }
}
