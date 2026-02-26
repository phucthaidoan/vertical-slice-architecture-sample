using Microsoft.AspNetCore.Http;
using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Shared.Api;

public static class ApiResults
{
    public static IResult ToApiResponse(Result result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(ApiResponse.SuccessResponse());
        }

        return ErrorToResult(result.Error);
    }

    public static IResult ToApiResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(ApiResponse<T>.SuccessResponse(result.Value));
        }

        return ErrorToResult(result.Error);
    }

    public static IResult FromError(Error error) => ErrorToResult(error);

    private static IResult ErrorToResult(Error error)
    {
        var errors = new List<ApiError> { new(error.Code, error.Description) };
        var response = ApiResponse.FailureResponse(error.Description, errors);

        return error.Type switch
        {
            ErrorType.Validation => Results.BadRequest(response),
            ErrorType.NotFound => Results.NotFound(response),
            ErrorType.Conflict => Results.Conflict(response),
            ErrorType.Forbidden => Results.Json(response, statusCode: StatusCodes.Status403Forbidden),
            ErrorType.Unauthorized => Results.Json(response, statusCode: StatusCodes.Status401Unauthorized),
            ErrorType.TooManyRequests => Results.Json(response, statusCode: StatusCodes.Status429TooManyRequests),
            ErrorType.ServiceUnavailable => Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable),
            _ => Results.Json(response, statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}

