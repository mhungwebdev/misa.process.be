using Microsoft.Extensions.Configuration;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.DAL.Repository
{
    public class DepartmentRepository : BaseRepository<Department>,IDepartmentRepository
    {
        public DepartmentRepository(IConfiguration configuration):base(configuration)
        {

        }
    }
}
