using PetFamily.Domain.Shared.Models;

namespace PetFamily.API.Contracts.Shared;

public class Envelope
{
    private Envelope(object? result, Error? error)
    {
        Result = result;
        ErrorCode = error?.Code;
        ErrorMessage = error?.Message;
    }

    public object? Result { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    public DateTimeOffset Created => DateTimeOffset.UtcNow;

    public static Envelope Ok(object? result = null) =>
        new (result, null);

    public static Envelope Error(Error? error = null) =>
        new(null, error);
}