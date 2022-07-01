using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace eIVF.Filters
{
    public class ApiResourceFilter : Attribute, IAsyncResourceFilter
    {
        private readonly string[] _headers;
        private string privateKey = "eIFVCodeKey23";

        public ApiResourceFilter(params string[] headers)
        {
            _headers = headers;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
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
                var sharedKey = context.HttpContext.Request.Headers["key"].ToString();
                if (sharedKey != "")
                {
                    string decryptedKey = EncryptionHelper.Decrypt(sharedKey, privateKey);

                    if (decryptedKey != "")
                    {
                        privateKey = decryptedKey;
                    }
                }
                var request = context.HttpContext.Request;
                //Decrypt Body
                if (context.HttpContext.Request.Method == HttpMethod.Post.ToString() || context.HttpContext.Request.Method == HttpMethod.Put.ToString())
                {
                    var stream = request.Body;
                    StreamReader streamRead = new StreamReader(request.Body);
                    string bodyContent = streamRead.ReadToEndAsync().GetAwaiter().GetResult();
                    if (bodyContent != "")
                    {
                        string decryptedInput = EncryptionHelper.Decrypt(bodyContent, privateKey);
                        if (decryptedInput != "")
                        {
                            var deserilizedFromJson = JsonConvert.SerializeObject(decryptedInput);
                            byte[] requestData = Encoding.ASCII.GetBytes(deserilizedFromJson);
                            context.HttpContext.Request.Body.Dispose();
                            context.HttpContext.Request.Body = new MemoryStream(requestData);
                            //context.HttpContext.Request.Body = new MemoryStream(requestData);
                            //context.HttpContext.Request.ContentLength = context.HttpContext.Request.Body.Length;
                        }
                    }
                }

            }
            ResourceExecutedContext executedContext = await next();
        }

        //private async Task<string> ReadBodyAsString(HttpRequest request)
        //{
        //    var initialBody = request.Body; // Workaround

        //    try
        //    {
          
        //       using (StreamReader reader = new StreamReader(
        //             request.Body,
        //             encoding: Encoding.UTF8,   
        //             detectEncodingFromByteOrderMarks: false
        //           ))
        //        {
        //            string text = await reader.ReadToEndAsync();
        //            return text;
        //        }
        //    }
        //    finally
        //    {
        //        request.Body = initialBody;
        //    }

        //    return string.Empty;
        //}


    }

}
