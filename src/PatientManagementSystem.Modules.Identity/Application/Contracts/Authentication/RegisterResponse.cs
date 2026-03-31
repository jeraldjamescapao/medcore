namespace PatientManagementSystem.Modules.Identity.Application.Contracts.Authentication;

public sealed record RegisterResponse(
    Guid UserId,
    string Email,
    string FullName,
    IList<string> Roles,
    string AccessToken);