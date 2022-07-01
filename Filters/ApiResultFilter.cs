using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eIVF.Filters
{
    public class ApiResultFilter : Attribute, IAsyncResultFilter
    {
        private readonly string[] _headers;
        private string privateKey = "eIFVCodeKey23";

        public ApiResultFilter(params string[] headers)
        {
            _headers = headers;
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {
            
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {

           
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (_headers == null) return;
            if (!_headers.All(m => context.HttpContext.Request.Headers.ContainsKey(m)))
            {
                context.Result = new JsonResult(
                    new { Error = "Headers Missing" }
                )
                { StatusCode = 400 };

            }
            else
            {
                context.HttpContext.Response.Headers.Keys.Add("key");
                context.HttpContext.Response.Headers["key"] = EncryptionHelper.Encrypt(privateKey, privateKey);
            }

            var result = context.Result as ObjectResult;
            if (result?.Value != null)
            {
                string encryptedContent = EncryptionHelper.Encrypt(JsonConvert.SerializeObject(result.Value), privateKey);
                if (encryptedContent != "")
                {
                    result.Value = encryptedContent;
                }
            }
            ResultExecutedContext resultContext = await next();

        }
    }
}
