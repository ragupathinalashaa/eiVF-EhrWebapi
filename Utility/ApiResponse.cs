namespace EhrWebApi.Utility
{
    public class ApiResponse
    {
        public ApiResponse(int statuscode, string messagePharse, dynamic data)
        {
            StatusCode = statuscode;
            Message = messagePharse;
            Data = data;
        }
        public ApiResponse(int statuscode)
        {
            StatusCode = statuscode;
        }
        public ApiResponse(int statuscode, dynamic data)
        {
            StatusCode = statuscode;
            Data = data;
        }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public dynamic? Data { get; set; }

    }
}
