using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PROCESS.BLL.Interfaces;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.DAL.Interfaces;

namespace MISA.PROCESS.API.Controllers
{
    public class JobPositionsController : MISABaseController<JobPosition>
    {
        public JobPositionsController(IJobPositionRepository jobPositionRepository, IJobPositionService jobPositionService):base(jobPositionRepository, jobPositionService)
        {

        }
    }
}
