namespace PatientManagementSystem.Modules.Identity.Application.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PatientManagementSystem.Common.Results;
using PatientManagementSystem.Modules.Identity.Application.Abstractions.Authentication;
using PatientManagementSystem.Modules.Identity.Application.Contracts.Authentication;
using PatientManagementSystem.Modules.Identity.Configuration;
using PatientManagementSystem.Modules.Identity.Domain.Users;
using PatientManagementSystem.Modules.Identity.Domain.Tokens;

internal sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtSettings.Value;
    }
    
    public async Task<Result<RegisterResponse>> RegisterAsync(
        RegisterRequest request, CancellationToken ct = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Result<RegisterResponse>.Conflict(AuthErrors.EmailAlreadyRegistered);

        var user = ApplicationUser.Create(
            request.Email,
            request.FirstName,
            request.LastName,
            request.BirthDate,
            createdBy: ApplicationUser.SelfRegisteredActor);
        
        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result<RegisterResponse>.Internal(AuthErrors.RegistrationFailed);
        
        var roleResult = await _userManager.AddToRoleAsync(user, Domain.Roles.IdentityRoles.Patient);
        if (!roleResult.Succeeded)
            return Result<RegisterResponse>.Internal(AuthErrors.RoleAssignmentFailed);

        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, rawRefreshToken) = await IssueTokenPairAsync(user, roles, ct);
        
        return Result<RegisterResponse>.Success(new RegisterResponse(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            accessToken)
        {
            RawRefreshToken = rawRefreshToken
        });
    }

    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result<LoginResponse>.Unauthorized(AuthErrors.InvalidCredentials);

        if (!user.IsActive)
            return Result<LoginResponse>.Unauthorized(AuthErrors.AccountDeactivated);

        if (!user.EmailConfirmed)
            return Result<LoginResponse>.Unauthorized(AuthErrors.EmailNotConfirmed);

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            return Result<LoginResponse>.Unauthorized(AuthErrors.InvalidCredentials);

        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, rawRefreshToken) = await IssueTokenPairAsync(user, roles, ct);
        
        return Result<LoginResponse>.Success(new LoginResponse(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            accessToken)
        {
            RawRefreshToken = rawRefreshToken
        });
    }

    public async Task<Result<RefreshResponse>> RefreshAsync(
        string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result<RefreshResponse>.Unauthorized(AuthErrors.InvalidRefreshToken);

        var existingToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, ct);
        if (existingToken is null)
            return Result<RefreshResponse>.Unauthorized(AuthErrors.InvalidRefreshToken);

        if (!existingToken.IsActive)
        {
            // A revoked token that was already replaced is a reuse signal.
            // Someone is replaying an old token — likely stolen (theft alert!).
            // Revoke the entire family to protect the user.
            if (existingToken.ReplacedByTokenId is not null)
            {
                await _refreshTokenRepository.RevokeAllForUserAsync(existingToken.UserId, ct);
                return Result<RefreshResponse>.Unauthorized(AuthErrors.TokenReuseDetected);
            }

            return Result<RefreshResponse>.Unauthorized(AuthErrors.TokenExpiredOrRevoked);
        }
        
        var user = await _userManager.FindByIdAsync(existingToken.UserId.ToString());
        if (user is null || !user.IsActive)
            return Result<RefreshResponse>.Unauthorized(AuthErrors.UserNotFoundOrInactive);

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var newRawRefreshToken = _jwtTokenService.GenerateRefreshToken();
        
        var newRefreshToken = RefreshToken.Create(
            user.Id,
            existingToken.FamilyId,
            newRawRefreshToken,
            DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays));

        existingToken.Revoke();
        existingToken.MarkReplacedBy(newRefreshToken.Id);
        
        await _refreshTokenRepository.AddAsync(newRefreshToken, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);
        
        return Result<RefreshResponse>.Success(new RefreshResponse(newAccessToken)
        {
            RawRefreshToken = newRawRefreshToken
        });
    }

    public async Task<Result<bool>> LogoutAsync(
        string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result<bool>.Success(true);
        
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, ct);
        if (existingToken is null || !existingToken.IsActive)
            return Result<bool>.Success(true);
        
        existingToken.Revoke();
        await _refreshTokenRepository.SaveChangesAsync(ct);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> LogoutAllAsync(
        Guid userId, CancellationToken ct = default)
    {
        await _refreshTokenRepository.RevokeAllForUserAsync(userId, ct);
        return Result<bool>.Success(true);
    }
    
    private async Task<(string AccessToken, string RawRefreshToken)> IssueTokenPairAsync(
        ApplicationUser user,
        IList<string> roles,
        CancellationToken ct)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var rawRefreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(
            user.Id,
            Guid.NewGuid(),
            rawRefreshToken,
            DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays));

        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        return (accessToken, rawRefreshToken);
    }
}