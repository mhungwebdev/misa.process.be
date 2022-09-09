using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PROCESS.BLL.Interfaces.InterfaceService;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.DAL.Interfaces.InterfaceRepository;

namespace MISA.PROCESS.API.Controllers
{
    public class DepartmentsController : MISABaseController<Department>
    {
        public DepartmentsController(IDepartmentRepository departmentRepository,IDepartmentService departmentService):base(departmentRepository,departmentService)
        {

        }
    }
}
