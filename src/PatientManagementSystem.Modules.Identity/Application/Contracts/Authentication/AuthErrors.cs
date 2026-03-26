namespace PatientManagementSystem.Modules.Identity.Application.Contracts.Authentication;

using PatientManagementSystem.Common.Results;

public static class AuthErrors
{
    public static readonly ResultError EmailAlreadyRegistered =
        new("IDENTITY_AUTH_EMAIL_ALREADY_REGISTERED", "Email is already registered.");

    public static readonly ResultError InvalidCredentials =
        new("IDENTITY_AUTH_INVALID_CREDENTIALS", "Invalid credentials.");

    public static readonly ResultError AccountDeactivated =
        new("IDENTITY_AUTH_ACCOUNT_DEACTIVATED", "Account is deactivated.");

    public static readonly ResultError InvalidRefreshToken =
        new("IDENTITY_AUTH_INVALID_REFRESH_TOKEN", "Invalid refresh token.");

    public static readonly ResultError TokenReuseDetected =
        new("IDENTITY_AUTH_TOKEN_REUSE_DETECTED", "Token reuse detected. All sessions revoked.");

    public static readonly ResultError TokenExpiredOrRevoked =
        new("IDENTITY_AUTH_TOKEN_EXPIRED_OR_REVOKED", "Refresh token is expired or revoked.");

    public static readonly ResultError UserNotFoundOrInactive =
        new("IDENTITY_AUTH_USER_NOT_FOUND_OR_INACTIVE", "User not found or deactivated.");

    public static readonly ResultError TokenAlreadyRevoked =
        new("IDENTITY_AUTH_TOKEN_ALREADY_REVOKED", "Token is already revoked or expired.");

    public static readonly ResultError RoleAssignmentFailed =
        new("IDENTITY_AUTH_ROLE_ASSIGNMENT_FAILED", "Failed to assign role.");
}