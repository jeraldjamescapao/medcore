namespace MedCorVis.Modules.Users.Application.Errors;

using MedCorVis.Common.Results;

public static class UserErrors
{
    public static readonly ResultError UserNotFound =
        new("USERS_USER_NOT_FOUND", "User not found.");
    
    public static readonly ResultError InvalidToken =
        new("USERS_INVALID_TOKEN", "Invalid or missing authentication token.");
    
    public static readonly ResultError UnsupportedCulture =
        new("USERS_UNSUPPORTED_CULTURE", "The specified culture is not supported.");
    
    public static readonly ResultError CultureUpdateFailed =
        new("USERS_CULTURE_UPDATE_FAILED", "Failed to update culture.");
    
    public static readonly ResultError ProfileUpdateFailed =
        new("USERS_PROFILE_UPDATE_FAILED", "Failed to update user profile.");

    public static readonly ResultError PhoneUpdateFailed =
        new("USERS_PHONE_UPDATE_FAILED", "Failed to update phone number.");
    
    public static readonly ResultError DeletionRequestAlreadyPending =
        new("USERS_DELETION_REQUEST_ALREADY_PENDING", "A deletion request is already pending.");

    public static readonly ResultError NoDeletionRequestPending =
        new("USERS_NO_DELETION_REQUEST_PENDING", "No deletion request is pending for this user.");

    public static readonly ResultError UserAlreadyDeleted =
        new("USERS_USER_ALREADY_DELETED", "This user has already been deleted.");

    public static readonly ResultError DeletionFailed =
        new("USERS_DELETION_FAILED", "Failed to complete user deletion.");
}