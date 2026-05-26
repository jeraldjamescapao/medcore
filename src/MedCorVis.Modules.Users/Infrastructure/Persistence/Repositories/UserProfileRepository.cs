namespace MedCorVis.Modules.Users.Infrastructure.Persistence.Repositories;

using MedCorVis.Modules.Users.Application.Abstractions;
using MedCorVis.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;

internal sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly UsersDbContext _context;

    public UserProfileRepository(UsersDbContext context)
    {
        _context = context;
    }
    
    public async Task<UserProfile?> GetByUserIdAsync(
        Guid userId, CancellationToken ct = default)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);
    }
    
    public async Task<IReadOnlyList<UserProfile>> GetByUserIdsAsync(
        IReadOnlyList<Guid> userIds, CancellationToken ct = default)
    {
        return await _context.UserProfiles
            .AsNoTracking()
            .Where(p => userIds.Contains(p.UserId))
            .ToListAsync(ct);
    }
    
    public async Task AddAsync(UserProfile profile, CancellationToken ct = default)
    {
        await _context.UserProfiles.AddAsync(profile, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}