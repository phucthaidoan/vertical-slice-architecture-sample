namespace VerticalSliceBoilerplate.Shared;

public sealed record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);

    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Forbidden(string code, string description) =>
        new(code, description, ErrorType.Forbidden);

    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);

    public static Error TooManyRequests(string code, string description) =>
        new(code, description, ErrorType.TooManyRequests);

    public static Error ServiceUnavailable(string code, string description) =>
        new(code, description, ErrorType.ServiceUnavailable);

    public override string ToString()
    {
        return $"{Code}|{Description}|{(int)Type}";
    }

    public static Error? FromString(string errorString)
    {
        if (string.IsNullOrWhiteSpace(errorString))
            return null;

        var parts = errorString.Split('|', 3, StringSplitOptions.TrimEntries);
        if (parts.Length < 2)
            return null;

        var code = parts[0];
        var description = parts[1];
        var type = ErrorType.Failure;

        if (parts.Length == 3 && int.TryParse(parts[2], out var typeValue) && Enum.IsDefined(typeof(ErrorType), typeValue))
        {
            type = (ErrorType)typeValue;
        }

        return new Error(code, description, type);
    }
}

