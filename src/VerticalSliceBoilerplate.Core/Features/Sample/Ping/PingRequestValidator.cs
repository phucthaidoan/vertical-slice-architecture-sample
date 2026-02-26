using FluentValidation;

namespace VerticalSliceBoilerplate.Core.Features.Sample.Ping;

public sealed class PingRequestValidator : AbstractValidator<PingRequest>
{
    public PingRequestValidator()
    {
        RuleFor(x => x.Message)
            .MaximumLength(100)
            .WithMessage("Message cannot exceed 100 characters.");
    }
}

