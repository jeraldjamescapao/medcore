namespace MedCore.Modules.Identity.Infrastructure.Persistence;

using Microsoft.Extensions.Logging;

internal static class AdminUserSeederLogMessages
{
    public static readonly Action<ILogger, string, Exception?> AdminUserSeedingStarted =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(5005, "AdminUserSeedingStarted"),
            "Seeding admin user '{Email}'.");

    public static readonly Action<ILogger, string, Exception?> AdminUserAlreadyExists =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(5006, "AdminUserAlreadyExists"),
            "Admin user '{Email}' already exists. Skipping.");

    public static readonly Action<ILogger, string, Exception?> AdminUserSeededSuccessfully =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(5007, "AdminUserSeededSuccessfully"),
            "Admin user '{Email}' seeded successfully.");

    public static readonly Action<ILogger, string, string, Exception?> AdminUserSeedFailed =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(5008, "AdminUserSeedFailed"),
            "Failed to seed admin user '{Email}': {Errors}.");
}