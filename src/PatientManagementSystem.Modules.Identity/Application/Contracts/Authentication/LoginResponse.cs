namespace PatientManagementSystem.Modules.Identity.Application.Contracts.Authentication;

public sealed record LoginResponse(
    Guid UserId,
    string Email,
    string FullName,
    IList<string> Roles,
    string AccessToken);