namespace PatientManagementSystem.Modules.Identity.Application.Abstractions.Email;

using PatientManagementSystem.Modules.Identity.Domain.Users;

public interface IIdentityEmailService
{
    Task SendConfirmationEmailAsync(
        ApplicationUser user, string encodedToken, CancellationToken ct = default);
}