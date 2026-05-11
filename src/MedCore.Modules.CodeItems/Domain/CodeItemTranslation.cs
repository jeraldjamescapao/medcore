namespace MedCore.Modules.CodeItems.Domain;

using MedCore.Common.Auditing;
using MedCore.Common.Exceptions;
using MedCore.Common.Localization;

internal sealed class CodeItemTranslation : IAuditableEntity
{
    public const string EntityTypeCategory = "Category";
    public const string EntityTypeItem = "Item";
    public const int EntityTypeMaxLength = 20;
    public const int CultureMaxLength = 10;
    public const int LabelMaxLength = 200;
    
    public long Id { get; private set; }
    public string EntityType { get; private set; } = null!;
    public long EntityId { get; private set; }
    public string Culture { get; private set; } = null!;
    public string Label { get; private set; } = null!;
    public bool IsActive { get; private set; }
    
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTimeOffset? ModifiedAtUtc { get; private set; }
    public string? ModifiedBy { get; private set; }
    
    private CodeItemTranslation(
        string entityType,
        long entityId,
        string culture,
        string label,
        string createdBy)
    {
        EntityType = entityType;
        EntityId = entityId;
        Culture = culture;
        Label = label;
        IsActive = true;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }
    
    public static CodeItemTranslation Create(
        string entityType,
        long entityId,
        string culture,
        string label,
        string createdBy)
    {
        if (entityType != EntityTypeCategory && entityType != EntityTypeItem)
            throw new DomainException("DOMAIN_CODEITEMTRANSLATION_INVALID_ENTITY_TYPE",
                $"EntityType must be '{EntityTypeCategory}' or '{EntityTypeItem}'.");
        
        if (entityId <= 0)
            throw new DomainException("DOMAIN_CODEITEMTRANSLATION_INVALID_ENTITY_ID", "EntityId is required.");
        
        if (string.IsNullOrWhiteSpace(culture))
            throw new DomainException("DOMAIN_CODEITEMTRANSLATION_INVALID_CULTURE", "Culture is required.");
        
        var trimmedCulture = culture.Trim();
        
        if (!SupportedCultures.All.Contains(trimmedCulture))
            throw new DomainException("DOMAIN_CODEITEMTRANSLATION_INVALID_CULTURE", "Unsupported culture.");
        
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainException("DOMAIN_CODEITEMTRANSLATION_INVALID_LABEL", "Label is required.");
        
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new DomainException("DOMAIN_CODEITEMTRANSLATION_INVALID_CREATED_BY", "CreatedBy is required.");

        return new CodeItemTranslation(entityType, entityId, trimmedCulture, label.Trim(), createdBy);
    }
    
    public void Update(string label, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainException("DOMAIN_CODEITEMTRANSLATION_INVALID_LABEL", "Label is required.");
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new DomainException("DOMAIN_CODEITEMTRANSLATION_INVALID_MODIFIED_BY", "ModifiedBy is required.");

        Label = label.Trim();
        IsActive = true;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
    }
    
    public void Deactivate(string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new DomainException("DOMAIN_CODEITEMTRANSLATION_INVALID_MODIFIED_BY", "ModifiedBy is required.");

        IsActive = false;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
    }
}