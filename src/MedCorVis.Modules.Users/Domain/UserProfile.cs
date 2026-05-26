namespace MedCorVis.Modules.Users.Domain;

using MedCorVis.Common.Auditing;
using MedCorVis.Common.Domain;

public sealed class UserProfile : IAuditableEntity, IDeletableEntity
{
    #region Constants

    public const int FirstNameMinLength = 2;
    public const int FirstNameMaxLength = 100;
    public const int LastNameMinLength  = 2;
    public const int LastNameMaxLength  = 100;

    #endregion
    
    #region Properties

    public Guid   Id        { get; private set; }
    public Guid   UserId    { get; private set; }

    public string  FirstName { get; private set; } = null!;
    public string  LastName  { get; private set; } = null!;
    public DateOnly BirthDate { get; private set; }

    // Soft delete
    public bool            IsDeleted    { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }
    public string?         DeletedBy    { get; private set; }

    // Audit
    public DateTimeOffset  CreatedAtUtc  { get; private set; }
    public string          CreatedBy     { get; private set; } = null!;
    public DateTimeOffset? ModifiedAtUtc { get; private set; }
    public string?         ModifiedBy    { get; private set; }

    // Computed
    public string FullName          => $"{FirstName} {LastName}";
    public string FullNameInverted  => $"{LastName}, {FirstName}";
    public string AbbreviatedName   =>
        FirstName.Length > 0 ? $"{FirstName[0]}. {LastName}" : LastName;

    #endregion
    
    #region Constructors

    private UserProfile() { }

    private UserProfile(
        Guid    userId,
        string  firstName,
        string  lastName,
        DateOnly birthDate,
        string  createdBy)
    {
        Id        = Guid.NewGuid();
        UserId    = userId;
        FirstName = firstName;
        LastName  = lastName;
        BirthDate = birthDate;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        CreatedBy    = createdBy;
    }

    #endregion
    
    #region Factory

    public static UserProfile Create(
        Guid    userId,
        string  firstName,
        string  lastName,
        DateOnly birthDate,
        string  createdBy)
    {
        DomainGuards.RequireNonEmptyGuid(
            userId, "DOMAIN_USERPROFILE_INVALID_USER_ID", "UserId cannot be empty.");

        var trimmedFirstName = DomainGuards.RequireValidLengthRange(
            firstName, 
            "DOMAIN_USERPROFILE_INVALID_FIRST_NAME", 
            $"FirstName must be between {FirstNameMinLength} and {FirstNameMaxLength} characters.", 
            FirstNameMinLength, 
            FirstNameMaxLength);

        var trimmedLastName = DomainGuards.RequireValidLengthRange(
            lastName, 
            "DOMAIN_USERPROFILE_INVALID_FIRST_NAME", 
            $"LastName must be between {LastNameMinLength} and {LastNameMaxLength} characters.", 
            LastNameMinLength, 
            LastNameMaxLength);
        
        var trimmedCreatedBy = DomainGuards.RequireNonEmpty(
            createdBy, "DOMAIN_USERPROFILE_INVALID_CREATED_BY", "CreatedBy is required.");

        DomainGuards.RequirePastOrPresentDate(birthDate, 
            "DOMAIN_USER_INVALID_BIRTH_DATE", "BirthDate cannot be in the future.");

        return new UserProfile(
            userId,
            trimmedFirstName,
            trimmedLastName,
            birthDate,
            trimmedCreatedBy);
    }

    #endregion
    
    #region Methods
    
    public void UpdateProfile(
        string  firstName,
        string  lastName,
        DateOnly birthDate,
        string  modifiedBy)
    {
        var trimmedFirstName = DomainGuards.RequireValidLengthRange(
            firstName, 
            "DOMAIN_USERPROFILE_INVALID_FIRST_NAME", 
            $"FirstName must be between {FirstNameMinLength} and {FirstNameMaxLength} characters.", 
            FirstNameMinLength, 
            FirstNameMaxLength);

        var trimmedLastName = DomainGuards.RequireValidLengthRange(
            lastName, 
            "DOMAIN_USERPROFILE_INVALID_FIRST_NAME", 
            $"LastName must be between {LastNameMinLength} and {LastNameMaxLength} characters.", 
            LastNameMinLength, 
            LastNameMaxLength);
        
        var trimmedModifiedBy = DomainGuards.RequireNonEmpty(
            modifiedBy, "DOMAIN_USERPROFILE_INVALID_MODIFIED_BY", "ModifiedBy is required.");

        DomainGuards.RequirePastOrPresentDate(birthDate, 
            "DOMAIN_USER_INVALID_BIRTH_DATE", "BirthDate cannot be in the future.");

        var nameChanged      = trimmedFirstName != FirstName || trimmedLastName != LastName;
        var birthDateChanged = birthDate != BirthDate;

        if (!nameChanged && !birthDateChanged) return;

        if (nameChanged)
        {
            FirstName = trimmedFirstName;
            LastName  = trimmedLastName;
        }

        if (birthDateChanged)
            BirthDate = birthDate;

        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy    = trimmedModifiedBy;
    }
    
    public void Anonymise(string deletedBy)
    {
        var trimmedDeletedBy = DomainGuards.RequireNonEmpty(
            deletedBy, "DOMAIN_USERPROFILE_INVALID_DELETED_BY", "DeletedBy is required.");

        FirstName    = "Deleted";
        LastName     = "User";
        IsDeleted    = true;
        DeletedAtUtc = DateTimeOffset.UtcNow;
        DeletedBy    = trimmedDeletedBy;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy    = trimmedDeletedBy;
    }
    
    #endregion   
}