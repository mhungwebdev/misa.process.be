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
    public class RoleRepository : BaseRepository<Role>,IRoleRepository
    {
        public RoleRepository(IConfiguration configuration):base(configuration)
        {

        }
    }
}
