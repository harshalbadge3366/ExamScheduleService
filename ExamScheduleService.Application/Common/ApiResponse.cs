namespace ExamScheduleService.Application.Common;

/// <summary>
/// Defines the standard API response envelope returned by the microservice.
/// </summary>
/// <typeparam name="T">The response payload type.</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the response message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the response payload.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Creates a successful response envelope.
    /// </summary>
    /// <param name="data">The response payload.</param>
    /// <param name="message">The response message.</param>
    /// <returns>A successful API response.</returns>
    public static ApiResponse<T> Ok(T data, string message = "Request completed successfully.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Creates a failed response envelope.
    /// </summary>
    /// <param name="message">The response message.</param>
    /// <returns>A failed API response.</returns>
    public static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}
