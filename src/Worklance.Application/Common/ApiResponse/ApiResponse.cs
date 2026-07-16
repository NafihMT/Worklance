using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Worklance.Application.Common.ApiResponse;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResponse<T> Success(
        T data,
        string message = "Success",
        int statusCode = StatusCodes.Status200OK)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Success(
        string message = "Success",
        int statusCode = StatusCodes.Status200OK)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Message = message
        };
    }

    public static ApiResponse<T> Failure(
        string message,
        int statusCode = StatusCodes.Status400BadRequest,
        IEnumerable<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };
    }
}