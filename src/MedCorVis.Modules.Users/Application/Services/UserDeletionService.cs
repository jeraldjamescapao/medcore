namespace MedCorVis.Modules.Users.Application.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedCorVis.Common.Results;
using MedCorVis.Common.Services;
using MedCorVis.Modules.Identity.Domain.Users;
using MedCorVis.Modules.Users.Application.Abstractions;
using MedCorVis.Modules.Users.Application.Contracts.Responses;
using MedCorVis.Modules.Users.Application.Errors;
using MedCorVis.Modules.Users.Application.Logging;

// Note: This service queries ApplicationUser directly via UserManager<ApplicationUser>.
// The Users module has no dedicated DbContext or repository — it shares the Identity
// module's user store through UserManager, which is the established pattern here.
// Future: When User and UserProfile are split into separate tables, this service
// will query UserProfile via a dedicated Users repository instead of UserManager.Users.
// GetPendingDeletionRequestsAsync will be the first method to migrate.
internal sealed class UserDeletionService : IUserDeletionService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UserDeletionService> _logger;

    public UserDeletionService(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService,
        ILogger<UserDeletionService> logger)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
        _logger = logger;
    }
    
    public async Task<Result<bool>> RequestDeletionAsync(CancellationToken ct = default)
    {
        var userId = Guid.Parse(_currentUserService.UserId);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            UserLogMessages.GetCurrentUserNotFound(_logger, userId, null);
            return Result<bool>.NotFound(UserErrors.UserNotFound);
        }

        if (user.DeletionRequestedAtUtc.HasValue)
        {
            UserLogMessages.DeletionRequestAlreadyPending(_logger, userId, null);
            return Result<bool>.Conflict(UserErrors.DeletionRequestAlreadyPending);
        }

        user.RequestDeletion();

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            UserLogMessages.UserDeletionFailed(_logger, userId, null);
            return Result<bool>.Internal(UserErrors.DeletionFailed);
        }

        UserLogMessages.DeletionRequestSubmitted(_logger, userId, null);
        return Result<bool>.Success(true);
    }
    
    public async Task<Result<bool>> CancelDeletionRequestAsync(CancellationToken ct = default)
    {
        var userId = Guid.Parse(_currentUserService.UserId);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            UserLogMessages.GetCurrentUserNotFound(_logger, userId, null);
            return Result<bool>.NotFound(UserErrors.UserNotFound);
        }

        if (!user.DeletionRequestedAtUtc.HasValue)
        {
            UserLogMessages.NoDeletionRequestPending(_logger, userId, null);
            return Result<bool>.UnprocessableEntity(UserErrors.NoDeletionRequestPending);
        }

        user.CancelDeletionRequest();

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            UserLogMessages.UserDeletionFailed(_logger, userId, null);
            return Result<bool>.Internal(UserErrors.DeletionFailed);
        }

        UserLogMessages.DeletionRequestCancelled(_logger, userId, null);
        return Result<bool>.Success(true);
    }
    
    public async Task<Result<IReadOnlyList<DeletionRequestResponse>>> GetPendingDeletionRequestsAsync(
        CancellationToken ct = default)
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.DeletionRequestedAtUtc != null)
            .OrderBy(u => u.DeletionRequestedAtUtc)
            .ToListAsync(ct);

        var response = users
            .Select(u => new DeletionRequestResponse(
                u.Id,
                u.FullName,
                u.Email!,
                u.DeletionRequestedAtUtc!.Value))
            .ToList();

        return Result<IReadOnlyList<DeletionRequestResponse>>.Success(response);
    }
    
    public async Task<Result<bool>> ExecuteDeletionAsync(
        Guid targetUserId, CancellationToken ct = default)
    {
        var actorId = Guid.Parse(_currentUserService.UserId);

        var user = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (user is null)
        {
            UserLogMessages.GetCurrentUserNotFound(_logger, targetUserId, null);
            return Result<bool>.NotFound(UserErrors.UserNotFound);
        }

        if (user.IsDeleted)
        {
            UserLogMessages.UserAlreadyDeleted(_logger, targetUserId, null);
            return Result<bool>.UnprocessableEntity(UserErrors.UserAlreadyDeleted);
        }

        user.Delete(actorId.ToString());

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            UserLogMessages.UserDeletionFailed(_logger, targetUserId, null);
            return Result<bool>.Internal(UserErrors.DeletionFailed);
        }

        UserLogMessages.UserDeletedSuccessfully(_logger, targetUserId, actorId, null);
        return Result<bool>.Success(true);
    }
}