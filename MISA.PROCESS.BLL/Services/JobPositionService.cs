using MISA.PROCESS.BLL.Interfaces;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.BLL.Services
{
    public class JobPositionService : BaseService<JobPosition>, IJobPositionService
    {
        public JobPositionService(IJobPositionRepository jobPositionRepository):base(jobPositionRepository)
        {

        }
    }
}
