using MedCorVis.Modules.Identity.Domain.Users;

namespace MedCorVis.Modules.Identity.Application.Abstractions.Email;

using Identity.Domain.Users;

internal interface IIdentityEmailService
{
    Task SendConfirmationEmailAsync(
        ApplicationUser user, 
        string encodedToken, 
        string culture,
        CancellationToken ct = default);
}