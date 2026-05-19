namespace MedCorVis.Modules.CodeItems.Domain;

using MedCorVis.Common.Auditing;
using MedCorVis.Common.Exceptions;
using MedCorVis.Common.Localization;

internal sealed class CodeItemTranslation : IAuditableEntity, IDeletableEntity
{
    public const string EntityTypeCategory  = "Category";
    public const string EntityTypeItem      = "Item";
    public const int    EntityTypeMaxLength = 20;
    public const int    CultureMaxLength    = 10;
    public const int    LabelMaxLength      = 200;
    public const int    DescriptionMaxLength = 500;

    public long             Id            { get; private set; }
    public string           EntityType    { get; private set; } = null!;
    public long             EntityId      { get; private set; }
    public string           Culture       { get; private set; } = null!;
    public string           Label         { get; private set; } = null!;
    public string?          Description   { get; private set; }

    // Business Control
    public bool             IsSystemDefined { get; private set; }

    // Visibility
    public bool             IsActive      { get; private set; }

    // Soft Delete
    public bool             IsDeleted     { get; private set; }
    public DateTimeOffset?  DeletedAtUtc  { get; private set; }
    public string?          DeletedBy     { get; private set; }

    // Audit
    public DateTimeOffset   CreatedAtUtc  { get; private set; }
    public string           CreatedBy     { get; private set; } = null!;
    public DateTimeOffset?  ModifiedAtUtc { get; private set; }
    public string?          ModifiedBy    { get; private set; }

    private CodeItemTranslation() { }

    private CodeItemTranslation(
        string entityType,
        long entityId,
        string culture,
        string label,
        string? description,
        bool isSystemDefined,
        string createdBy)
    {
        EntityType      = entityType;
        EntityId        = entityId;
        Culture         = culture;
        Label           = label;
        Description     = description;
        IsSystemDefined = isSystemDefined;
        IsActive        = true;
        IsDeleted       = false;
        CreatedAtUtc    = DateTimeOffset.UtcNow;
        CreatedBy       = createdBy;
    }

    public static CodeItemTranslation Create(
        string entityType,
        long entityId,
        string culture,
        string label,
        string? description,
        bool isSystemDefined,
        string createdBy)
    {
        if (entityType != EntityTypeCategory && entityType != EntityTypeItem)
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_ENTITY_TYPE",
                $"EntityType must be '{EntityTypeCategory}' or '{EntityTypeItem}'.");
        
        if (entityId <= 0)
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_ENTITY_ID",
                "EntityId must be greater than zero.");
        
        if (string.IsNullOrWhiteSpace(culture))
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_CULTURE",
                "Culture is required.");
        
        var trimmedCulture = culture.Trim();
        
        if (!SupportedCultures.All.Contains(trimmedCulture))
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_CULTURE",
                "Unsupported culture.");
        
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_LABEL",
                "Label is required.");
        
        var trimmedLabel = label.Trim();
        
        if (trimmedLabel.Length > LabelMaxLength)
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_LABEL",
                $"Label cannot exceed {LabelMaxLength} characters.");
        
        var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        
        if (trimmedDescription?.Length > DescriptionMaxLength)
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_DESCRIPTION",
                $"Description cannot exceed {DescriptionMaxLength} characters.");
        
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_CREATED_BY",
                "CreatedBy is required.");

        return new CodeItemTranslation(
            entityType,
            entityId,
            trimmedCulture,
            trimmedLabel,
            trimmedDescription,
            isSystemDefined,
            createdBy);
    }

    public void Update(string label, string? description, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_LABEL",
                "Label is required.");
        
        var trimmedLabel = label.Trim();
        
        if (trimmedLabel.Length > LabelMaxLength)
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_LABEL",
                $"Label cannot exceed {LabelMaxLength} characters.");
        
        var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        
        if (trimmedDescription?.Length > DescriptionMaxLength)
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_DESCRIPTION",
                $"Description cannot exceed {DescriptionMaxLength} characters.");
        
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_MODIFIED_BY",
                "ModifiedBy is required.");

        Label         = trimmedLabel;
        Description   = trimmedDescription;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy    = modifiedBy;
    }

    public void Deactivate(string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_MODIFIED_BY",
                "ModifiedBy is required.");

        if (!IsActive) return;

        IsActive      = false;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy    = modifiedBy;
    }
    
    public void Reactivate(string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_MODIFIED_BY",
                "ModifiedBy is required.");

        if (IsActive) return;

        IsActive      = true;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy    = modifiedBy;
    }

    public void Delete(string deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            throw new DomainException(
                "DOMAIN_CODEITEMTRANSLATION_INVALID_DELETED_BY",
                "DeletedBy is required.");

        if (IsDeleted) return;

        IsDeleted    = true;
        IsActive     = false;
        DeletedAtUtc = DateTimeOffset.UtcNow;
        DeletedBy    = deletedBy;
    }
}