namespace MedCorVis.Modules.Identity.Domain.Users;

using MedCorVis.Common.Domain;
using Microsoft.AspNetCore.Identity;
using MedCorVis.Common.Auditing;

public sealed class ApplicationUser : IdentityUser<Guid>, IAuditableEntity, IDeletableEntity
{
    #region Constants
    
    public const string SelfRegisteredActor = "Self";
    public const int EmailMaxLength = 256;
    public const int PreferredCultureMaxLength = 10;    
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 128;
    
    #endregion
    
    #region Properties
    
    // Account Preferences
    public string? PreferredCulture { get; private set; }
    
    // Visibility
    public bool IsActive { get; private set; } = true;
    
    // Soft Delete
    // DeletionRequestedAtUtc: user has requested deletion, pending processing
    // IsDeleted: deletion has been processed
    public bool             IsDeleted     { get; private set; }
    public DateTimeOffset?  DeletedAtUtc  { get; private set; }
    public string?          DeletedBy     { get; private set; }
    public DateTimeOffset?  DeletionRequestedAtUtc { get; private set; }
    
    // Audit
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? ModifiedAtUtc { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public string? ModifiedBy { get; private set; }

    #endregion
    
    #region Constructors
    
    private ApplicationUser() { }

    private ApplicationUser(
        string email,
        string createdBy,
        string? preferredCulture)
    {
        Id = Guid.NewGuid();
        
        Email = email;
        UserName = email;
        PreferredCulture = preferredCulture;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
        IsActive = true;
    }
    
    #endregion   
    
    #region Factory

    public static ApplicationUser Create(
        string email,
        string createdBy,
        string? preferredCulture = null)
    {
        var trimmedEmail = DomainGuards.RequireNonEmpty(
            email, "DOMAIN_USER_INVALID_EMAIL", "Email is required.");
        var trimmedCulture = DomainGuards.RequireValidCulture(
            preferredCulture, "DOMAIN_USER_INVALID_CULTURE", "Unsupported culture.");
        var trimmedCreatedBy = DomainGuards.RequireNonEmpty(
            createdBy, "DOMAIN_USER_INVALID_CREATED_BY", "CreatedBy is required.", 
            IAuditableEntity.CreatedByMaxLength);
        
        return new ApplicationUser(
            trimmedEmail,
            trimmedCreatedBy,
            trimmedCulture);
    }
    
    #endregion
    
    #region Methods
    
    public void UpdatePreferredCulture(string culture, string modifiedBy)
    {
        var trimmedCulture = DomainGuards.RequireValidCulture(
            culture, "DOMAIN_USER_INVALID_CULTURE", "Culture is required or unsupported.");
        var trimmedModifiedBy = DomainGuards.RequireNonEmpty(
            modifiedBy, "DOMAIN_USER_INVALID_MODIFIED_BY", "ModifiedBy is required.",
            IAuditableEntity.ModifiedByMaxLength);
        
        if (trimmedCulture == PreferredCulture) return;
        
        PreferredCulture = trimmedCulture;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy = trimmedModifiedBy;
    }
    
    public void Deactivate(string modifiedBy)
    {
        var trimmedModifiedBy = DomainGuards.RequireNonEmpty(
            modifiedBy, "DOMAIN_USER_INVALID_MODIFIED_BY", "ModifiedBy is required.",
            IAuditableEntity.ModifiedByMaxLength);
        
        if (!IsActive) return;
        
        IsActive = false;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy = trimmedModifiedBy;
    }
    
    public void Activate(string modifiedBy)
    {
        var trimmedModifiedBy = DomainGuards.RequireNonEmpty(
            modifiedBy, "DOMAIN_USER_INVALID_MODIFIED_BY", "ModifiedBy is required.",
            IAuditableEntity.ModifiedByMaxLength);
        
        if (IsActive) return;
        
        IsActive = true;
        ModifiedAtUtc = DateTimeOffset.UtcNow; 
        ModifiedBy = trimmedModifiedBy;
    }
    
    public void RequestDeletion()
    {
        if (IsDeleted) return;
        if (DeletionRequestedAtUtc.HasValue) return;

        DeletionRequestedAtUtc = DateTimeOffset.UtcNow;
        ModifiedAtUtc          = DateTimeOffset.UtcNow;
        ModifiedBy             = SelfRegisteredActor;
    }
    
    public void CancelDeletionRequest()
    {
        if (!DeletionRequestedAtUtc.HasValue) return;

        DeletionRequestedAtUtc = null;
        ModifiedAtUtc          = DateTimeOffset.UtcNow;
        ModifiedBy             = SelfRegisteredActor;
    }
    
    public void Delete(string deletedBy)
    {
        if (IsDeleted) return;

        var trimmedDeletedBy = DomainGuards.RequireNonEmpty(
            deletedBy, "DOMAIN_USER_INVALID_DELETED_BY", "DeletedBy is required.");

        Anonymise();

        IsDeleted              = true;
        DeletedAtUtc           = DateTimeOffset.UtcNow;
        DeletedBy              = trimmedDeletedBy;
        IsActive               = false;
        ModifiedAtUtc          = DateTimeOffset.UtcNow;
        ModifiedBy             = trimmedDeletedBy;
    }
    
    private void Anonymise()
    {
        Email               = $"deleted_{Id}@deleted.invalid";
        UserName            = $"deleted_{Id}@deleted.invalid";
        NormalizedEmail     = $"DELETED_{Id}@DELETED.INVALID";
        NormalizedUserName  = $"DELETED_{Id}@DELETED.INVALID";
        PhoneNumber         = null;
    }
    
    #endregion
}