using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.COMMON
{
    public class Task
    {
        public int? TaskID { get; set; }

        public string TaskTitle { get; set; }

        public DateTime DoneDate { get; set; }

        public DateTime StartDate { get; set; }

        public int Status { get; set; }

        public int State { get; set; }

        public int? TaskParentID { get; set; }
    }
}
