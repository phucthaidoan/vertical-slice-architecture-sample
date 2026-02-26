using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Core.Features.Sample.Errors;

public static class SampleErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Sample.NotFound", "Sample resource was not found.");

    public static readonly Error InvalidOperation =
        Error.Failure("Sample.InvalidOperation", "The sample operation is invalid.");
}

