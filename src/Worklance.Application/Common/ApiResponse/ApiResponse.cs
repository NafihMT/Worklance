using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Worklance.Application.Common.ApiResponse
{
    public abstract class ApiResponse
    {
        [JsonPropertyOrder(1)]
        public bool IsSuccess { get; protected set; }

        [JsonPropertyOrder(2)]
        public int StatusCode { get; protected set; }

        [JsonPropertyOrder(3)]
        public string Message { get; protected set; } = string.Empty;

    
        public static SuccessResponse Success(string message, int statusCode) => new SuccessResponse(message, statusCode);

        public static SuccessResponse Success(object data, int statusCode) => new SuccessResponse(data, statusCode);
        public static SuccessResponse Success(object data, string message, int statusCode) => new SuccessResponse(data, message, statusCode);
        public static SuccessResponse Success(object data, string fileBase64, string message, int statusCode) => new SuccessResponse(data, fileBase64, message, statusCode);

        public static FailureResponse Failure(string message, int statusCode) => new FailureResponse(message, statusCode);
        public static FailureResponse Failure(string message, int statusCode, IEnumerable<string> errors) => new FailureResponse(message, statusCode, errors);
    }

    public class SuccessResponse : ApiResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(4)]
        public object? Data { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(5)]
        public string? File { get; }

        public SuccessResponse(string message, int statusCode)
        {
            IsSuccess = true;
            StatusCode = statusCode;
            Message = message;
        }

        public SuccessResponse(object data, int statusCode)
        {
            IsSuccess = true;
            StatusCode = statusCode;
            Message = "Success";
            Data = data;
        }

        public SuccessResponse(object data, string message, int statusCode)
        {
            IsSuccess = true;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public SuccessResponse(object data, string fileBase64, string message, int statusCode)
        {
            IsSuccess = true;
            StatusCode = statusCode;
            Message = message;
            Data = data;
            File = fileBase64;
        }
    }

    public class FailureResponse : ApiResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(4)]
        public IEnumerable<string>? Errors { get; }

        public FailureResponse(string message, int statusCode)
        {
            IsSuccess = false;
            StatusCode = statusCode;
            Message = message;
        }

        public FailureResponse(string message, int statusCode, IEnumerable<string> errors)
        {
            IsSuccess = false;
            StatusCode = statusCode;
            Message = message;
            Errors = errors;
        }
    }
}