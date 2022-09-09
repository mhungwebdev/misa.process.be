using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PROCESS.BLL.Interfaces;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.DAL.Interfaces;

namespace MISA.PROCESS.API.Controllers
{
    public class DepartmentsController : MISABaseController<Department>
    {
        public DepartmentsController(IDepartmentRepository departmentRepository,IDepartmentService departmentService):base(departmentRepository,departmentService)
        {

        }
    }
}
