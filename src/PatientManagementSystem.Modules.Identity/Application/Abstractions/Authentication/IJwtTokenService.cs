namespace PatientManagementSystem.Modules.Identity.Application.Abstractions.Authentication;

using PatientManagementSystem.Modules.Identity.Domain.Users;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
}