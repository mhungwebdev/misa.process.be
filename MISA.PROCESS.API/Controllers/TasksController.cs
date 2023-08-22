using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PROCESS.COMMON;
using MISA.PROCESS.DAL.Interfaces;
using System.Threading.Tasks;

namespace MISA.PROCESS.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private IBaseRepository<COMMON.Task> _baseRepository;

        public TasksController(IBaseRepository<COMMON.Task> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        [HttpPost("save")]
        public async Task<bool> SaveTask(COMMON.Task task)
        {
            var result = false;

            using(var connection = _baseRepository.GetConnection())
            {
                var commandInsert = "INSERT INTO task (TaskTitle,Status,StartDate,DoneDate,TaskParentID) VALUES (@TaskTitle,@Status,@StartDate,@DoneDate,@TaskParentID);";
                var param = new Dictionary<string, object>() {
                    {"@TaskTitle",task.TaskTitle },
                    {"@Status", task.Status },
                    {"@StartDate", task.StartDate },
                    {"@DoneDate", task.DoneDate },
                    {"@TaskParentID", task.TaskParentID }
                };

                var res = await connection.ExecuteAsync(commandInsert, param, commandType: System.Data.CommandType.Text);
                result = res > 0;
            }

            return result;
        }

        [HttpGet("getAll")]
        public async Task<List<COMMON.Task>> GetAll()
        {
            var result = new List<COMMON.Task>();

            using (var connection = _baseRepository.GetConnection())
            {
                result = (List<COMMON.Task>)await connection.QueryAsync<COMMON.Task>("SELECT * FROM task", commandType: System.Data.CommandType.Text);
            }

            return result;
        }
    }

}
