using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Shared.Api;

public sealed record ApiError(string Code, string Message);

public sealed record ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ApiError> Errors { get; set; } = new();

    public static ApiResponse SuccessResponse(string message = "Success")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static ApiResponse FailureResponse(string message, List<ApiError>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<ApiError>()
        };
    }

    public static ApiResponse FromResult(Result result)
    {
        if (result.IsSuccess)
        {
            return SuccessResponse();
        }

        return FailureResponse(
            result.Error.Description,
            new List<ApiError> { new(result.Error.Code, result.Error.Description) }
        );
    }
}

public sealed record ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ApiError> Errors { get; set; } = new();

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> FailureResponse(string message, List<ApiError>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<ApiError>()
        };
    }

    public static ApiResponse<T> FromResult(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return SuccessResponse(result.Value);
        }

        return FailureResponse(
            result.Error.Description,
            new List<ApiError> { new(result.Error.Code, result.Error.Description) }
        );
    }
}

