using MISA.PROCESS.COMMON.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Exceptions
{
    /// <summary>
    /// Custome exception
    /// </summary>
    public class MISAException : Exception
    {
        #region contructor
        /// <summary>
        /// custome data trả về
        /// </summary>
        Dictionary<string,object> MISAdata = new Dictionary<string, object>();
        public MISAException(Dictionary<string, object> data)
        {
            MISAdata = data;
        }
        #endregion

        public override IDictionary Data {
            get {
                return MISAdata;
            }
        }

        public override string Message
        {
            get
            {
                return Resources.ExceptionMessage;
            }
        }
    }
}
