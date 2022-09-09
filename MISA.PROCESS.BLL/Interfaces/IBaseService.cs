using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.BLL.Interfaces
{
    public interface IBaseService<MISAEntity>
    {
        /// <summary>
        /// Thêm mới bản ghi có thực hiện nghiệp vụ
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">đối tượng muốn thêm mới</param>
        /// <returns>1 nếu thành công, 0 nếu thất bại</returns>
        int Insert(MISAEntity entity);

        /// <summary>
        /// Sửa một bản ghi có thực hiện nghiệp vụ
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">Đối tượng muốn update</param>
        /// <returns>1 nếu thành công, 0 nếu thất bại</returns>
        int Update(MISAEntity entity, Guid id);

        /// <summary>
        /// Thêm mới nhiều bản ghi
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="entities"></param>
        /// <returns>Số bản ghi thêm mới thành công</returns>
        int InsertMulti(List<MISAEntity> entities);
    }
}
