using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace VerticalSliceBoilerplate.Shared.Api.Endpoints;

public sealed class ValidationFilter<TRequest> : IEndpointFilter where TRequest : class
{
    private readonly IValidator<TRequest> _validator;

    public ValidationFilter(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (model is null)
        {
            return Results.BadRequest(ApiResponse.FailureResponse(
                "Invalid request payload.",
                new List<ApiError> { new("InvalidPayload", "The request payload could not be read.") }
            ));
        }

        var result = await _validator.ValidateAsync(model, context.HttpContext.RequestAborted);
        if (!result.IsValid)
        {
            var errors = result.Errors
                .Select(e => new ApiError(
                    string.IsNullOrWhiteSpace(e.PropertyName) ? "Validation" : e.PropertyName,
                    e.ErrorMessage))
                .ToList();

            return Results.BadRequest(ApiResponse.FailureResponse("Validation failed.", errors));
        }

        return await next(context);
    }
}

