namespace PatientManagementSystem.Modules.Identity.Configuration;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int AccessTokenExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationInDays { get; init; }
}