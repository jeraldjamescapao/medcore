namespace MedCorVis.Modules.Users.Application.Contracts.Responses;

public sealed record DeletionRequestResponse(
    Guid UserId,
    string FullName,
    string Email,
    DateTimeOffset DeletionRequestedAtUtc);