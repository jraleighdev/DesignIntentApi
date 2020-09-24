using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesignIntentApi.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public ApiResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request",
                401 => "Not Authorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}
