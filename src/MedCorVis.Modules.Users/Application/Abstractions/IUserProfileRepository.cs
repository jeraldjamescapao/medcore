namespace MedCorVis.Modules.Users.Application.Abstractions;

using MedCorVis.Modules.Users.Domain;

internal interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<UserProfile>> GetByUserIdsAsync(
        IReadOnlyList<Guid> userIds, CancellationToken ct = default);
    Task AddAsync(UserProfile profile, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}