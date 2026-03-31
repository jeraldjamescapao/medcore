using PatientManagementSystem.Common.Results;
using PatientManagementSystem.Modules.Identity.Application.Contracts.Authentication;

namespace PatientManagementSystem.Modules.Identity.Application.Services;

using PatientManagementSystem.Modules.Identity.Application.Abstractions.Authentication;

internal sealed class AuthService : IAuthService
{
    public Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<RefreshResponse>> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> LogoutAllAsync(Guid userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}