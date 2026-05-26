namespace MedCorVis.Modules.Users.Application.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MedCorVis.Common.Caching;
using MedCorVis.Common.Localization;
using MedCorVis.Common.Results;
using MedCorVis.Modules.Identity.Domain.Users;
using MedCorVis.Modules.Users.Application.Abstractions;
using MedCorVis.Modules.Users.Application.Contracts.Requests;
using MedCorVis.Modules.Users.Application.Contracts.Responses;
using MedCorVis.Modules.Users.Application.Errors;
using MedCorVis.Modules.Users.Application.Logging;
using MedCorVis.Modules.Users.Domain;

internal sealed class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserCultureCache _userCultureCache;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<UserService> _logger;
    
    public UserService(
        UserManager<ApplicationUser> userManager, 
        IUserCultureCache userCultureCache,
        IUserProfileRepository userProfileRepository,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _userCultureCache = userCultureCache;
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }
    
    public async Task<Result<UserResponse>> GetCurrentUserAsync(
        Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            UserLogMessages.GetCurrentUserNotFound(_logger, userId, null);
            return Result<UserResponse>.NotFound(UserErrors.UserNotFound);
        }

        var profile = await _userProfileRepository.GetByUserIdAsync(userId, ct);
        if (profile is null)
        {
            UserLogMessages.GetCurrentUserNotFound(_logger, userId, null);
            return Result<UserResponse>.NotFound(UserErrors.UserNotFound);
        }

        UserLogMessages.GetCurrentUserSucceeded(_logger, userId, null);

        return Result<UserResponse>.Success(MapToResponse(user, profile));
    }
    
    public async Task<Result<bool>> UpdateCultureAsync(
        Guid userId, string culture, CancellationToken ct = default)
    {
        if (!SupportedCultures.All.Contains(culture))
        {
            UserLogMessages.UpdateCultureUnsupported(_logger, userId, culture, null);
            return Result<bool>.Validation(UserErrors.UnsupportedCulture);
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            UserLogMessages.UpdateCultureUserNotFound(_logger, userId, null);
            return Result<bool>.NotFound(UserErrors.UserNotFound);
        }

        user.UpdatePreferredCulture(culture, userId.ToString());
        
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            UserLogMessages.UpdateCultureFailed(_logger, userId, null);
            return Result<bool>.Internal(UserErrors.CultureUpdateFailed);
        }

        _userCultureCache.SetCultureForUser(userId, culture);

        UserLogMessages.UpdateCultureSucceeded(_logger, userId, culture, null);

        return Result<bool>.Success(true);
    }
    
    public async Task<Result<UserResponse>> UpdateProfileAsync(
        Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            UserLogMessages.UpdateProfileUserNotFound(_logger, userId, null);
            return Result<UserResponse>.NotFound(UserErrors.UserNotFound);
        }

        var profile = await _userProfileRepository.GetByUserIdAsync(userId, ct);
        if (profile is null)
        {
            UserLogMessages.UpdateProfileUserNotFound(_logger, userId, null);
            return Result<UserResponse>.NotFound(UserErrors.UserNotFound);
        }

        profile.UpdateProfile(
            request.FirstName, request.LastName, request.BirthDate, userId.ToString());

        await _userProfileRepository.SaveChangesAsync(ct);

        UserLogMessages.UpdateProfileSucceeded(_logger, userId, null);

        return Result<UserResponse>.Success(MapToResponse(user, profile));
    }
    
    public async Task<Result<bool>> UpdatePhoneAsync(
        Guid userId, string? phoneNumber, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            UserLogMessages.UpdatePhoneUserNotFound(_logger, userId, null);
            return Result<bool>.NotFound(UserErrors.UserNotFound);
        }

        var updateResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
        if (!updateResult.Succeeded)
        {
            UserLogMessages.UpdatePhoneFailed(_logger, userId, null);
            return Result<bool>.Internal(UserErrors.PhoneUpdateFailed);
        }

        UserLogMessages.UpdatePhoneSucceeded(_logger, userId, null);

        return Result<bool>.Success(true);
    }

    private static UserResponse MapToResponse(ApplicationUser user, UserProfile profile)
    {
        return new UserResponse(
            user.Id,
            user.Email!,
            profile.FirstName,
            profile.LastName,
            profile.FullName,
            profile.BirthDate,
            user.PreferredCulture,
            user.PhoneNumber,
            user.IsActive,
            user.IsDeleted,
            user.DeletionRequestedAtUtc,
            user.CreatedAtUtc,
            user.ModifiedAtUtc);
    }
}