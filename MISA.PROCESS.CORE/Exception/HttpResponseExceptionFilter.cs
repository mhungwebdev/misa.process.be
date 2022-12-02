using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MISA.PROCESS.COMMON.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IActionFilter = Microsoft.AspNetCore.Mvc.Filters.IActionFilter;

namespace MISA.PROCESS.COMMON.Exceptions
{
    /// <summary>
    /// Tạo middleware exception
    /// </summary>
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception != null)
            {
                if (context.Exception is MISAException misaException)
                {
                    context.Result = new ObjectResult(misaException.Data)
                    {
                        StatusCode = 400
                    };

                    context.ExceptionHandled = true;
                }
                else
                {
                    var result = new
                    {
                        userMsg = Resources.Resources.CodeError,
                        devMsg = context.Exception.Message,
                        errorMsg = "",
                    };

                    context.Result = new ObjectResult(result)
                    {
                        StatusCode = 500
                    };

                    context.ExceptionHandled = true;
                }
            }
        }
    }
}
