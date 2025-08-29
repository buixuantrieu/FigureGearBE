using Microsoft.AspNetCore.Http;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public bool Success => StatusCode >= 200 && StatusCode < 300;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok( string message = "", T data = default) =>
        new() { StatusCode = StatusCodes.Status200OK, Message = message, Data = data };

    public static ApiResponse<T> Created( string message = "", T? data = default) =>
        new() { StatusCode = StatusCodes.Status201Created, Message = message, Data = data };

    public static ApiResponse<T> BadRequest(string message) =>
        new() { StatusCode = StatusCodes.Status400BadRequest, Message = message };

    public static ApiResponse<T> Unauthorized(string message) =>
        new() { StatusCode = StatusCodes.Status401Unauthorized, Message = message };

    public static ApiResponse<T> Forbidden(string message) =>
        new() { StatusCode = StatusCodes.Status403Forbidden, Message = message };

    public static ApiResponse<T> NotFound(string message) =>
        new() { StatusCode = StatusCodes.Status404NotFound, Message = message };

    public static ApiResponse<T> Custom(int statusCode, string message, T? data = default) =>
        new() { StatusCode = statusCode, Message = message, Data = data };
}
