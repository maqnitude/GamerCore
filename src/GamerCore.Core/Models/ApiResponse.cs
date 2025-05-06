using System.Text.Json.Serialization;

namespace GamerCore.Core.Models
{
    /// <summary>
    /// Standard API response format based on the Google JSON style guide.
    /// </summary>
    public class ApiResponse<T>
    {
        public T? Data { get; private set; }
        public ErrorDetail? Error { get; private set; }

        // Ensure the factory methods are used
        private ApiResponse() { }

        [JsonConstructor]
        public ApiResponse(T? data, ErrorDetail? error)
        {
            Data = data;
            Error = error;
        }

        /// <summary>
        /// Factory method to create a successful response.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiResponse<T> CreateSuccess(T? data)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Error = null
            };
        }

        /// <summary>
        /// Factory method to create an error response.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse<T> CreateError(int code, string message)
        {
            return new ApiResponse<T>
            {
                Data = default,
                Error = new ErrorDetail
                {
                    Code = code,
                    Message = message
                }
            };
        }

        public class ErrorDetail
        {
            public int Code { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}