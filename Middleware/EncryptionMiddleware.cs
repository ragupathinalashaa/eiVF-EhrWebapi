
using System.Security.Cryptography;
using System.Text;

namespace eIVF.Middleware
{
   
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate _next;
        public EncryptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {

            httpContext.Request.Body = DecryptStream(httpContext.Request.Body);
            httpContext.Response.Body = EncryptStream(httpContext.Response.Body);
            if (httpContext.Request.QueryString.HasValue)
            {
                string decryptedString = DecryptString(httpContext.Request.QueryString.Value.Substring(1));
                httpContext.Request.QueryString = new QueryString($"?{decryptedString}");
            }
            if (httpContext.Request.Headers.ContainsKey("Content-Type"))
                httpContext.Request.Headers["Content-Type"] = new Microsoft.Extensions.Primitives.StringValues("application/json");
            else
                httpContext.Request.Headers.Add("Content-Type", new Microsoft.Extensions.Primitives.StringValues("application/json"));

            await _next(httpContext);
            await httpContext.Request.Body.DisposeAsync();
            await httpContext.Response.Body.DisposeAsync();


        }

        private static string DecryptString(string cipherText)
        {
            Aes aes = GetEncryptionAlgorithm();
            byte[] buffer = Convert.FromBase64String(cipherText);

            using MemoryStream memoryStream = new MemoryStream(buffer);
            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }
        private static CryptoStream EncryptStream(Stream responseStream)
        {
            Aes aes = GetEncryptionAlgorithm();

            ToBase64Transform base64Transform = new ToBase64Transform();
            CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);

            return cryptoStream;
        }
        private static Stream DecryptStream(Stream cipherStream)
        {
            Aes aes = GetEncryptionAlgorithm();

            FromBase64Transform base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
            CryptoStream base64DecodedStream = new CryptoStream(cipherStream, base64Transform, CryptoStreamMode.Read);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream decryptedStream = new CryptoStream(base64DecodedStream, decryptor, CryptoStreamMode.Read);
            return decryptedStream;
        }
        private static Aes GetEncryptionAlgorithm()
        {
            Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes("#$@aVeryHard999Key_To_Encrypt!!!");
            //aes.Mode = CipherMode.CBC;
            //aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();

            return aes;
        }
     
    }
}
