namespace MedCorVis.Modules.Users.Application.Abstractions;

public interface IUserProfileService
{
    Task CreateProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        DateOnly birthDate,
        string createdBy,
        CancellationToken ct = default);

    Task AnonymiseProfileAsync(Guid userId, string deletedBy, CancellationToken ct = default);
}