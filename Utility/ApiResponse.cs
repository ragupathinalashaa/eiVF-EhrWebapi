namespace eIVF.Utility
{

    public class ApiResponse<TEntity>
    {
        public ApiResponse()
        {
        }
        public ApiResponse(int statuscode, string messagePharse, TEntity data)
        {
            StatusCode = statuscode;
            Message = messagePharse;
            Data = data;
        }
        public ApiResponse(int statuscode)
        {
            StatusCode = statuscode;
        }
        public ApiResponse(int statuscode, TEntity data)
        {
            StatusCode = statuscode;
            Data = data;
        }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public TEntity Data { get; set; }

    }
}
