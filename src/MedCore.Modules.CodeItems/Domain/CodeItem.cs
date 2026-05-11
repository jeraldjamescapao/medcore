namespace MedCore.Modules.CodeItems.Domain;

using MedCore.Common.Auditing;
using MedCore.Common.Exceptions;

internal sealed class CodeItem : IAuditableEntity
{
    public const int CodeMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    public long Id { get; private set; }
    public long CategoryId { get; private set; }
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
    
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTimeOffset? ModifiedAtUtc { get; private set; }
    public string? ModifiedBy { get; private set; }
    
    private CodeItem() { }

    private CodeItem(
        long categoryId, 
        string code, 
        string? description, 
        int sortOrder, 
        string createdBy)
    {
        CategoryId = categoryId;
        Code = code;
        Description = description;
        SortOrder = sortOrder;
        IsActive = true;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }
    
    public static CodeItem Create(
        long categoryId,
        string code,
        string? description,
        int sortOrder,
        string createdBy)
    {
        if (categoryId <= 0)
            throw new DomainException("DOMAIN_CODEITEM_INVALID_CATEGORY", "CategoryId must be greater than zero.");
        
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("DOMAIN_CODEITEM_INVALID_CODE", "Code is required.");
        
        var trimmedCode = code.Trim();
        
        if (trimmedCode.Length > CodeMaxLength)
            throw new DomainException("DOMAIN_CODEITEM_INVALID_CODE", $"Code cannot exceed {CodeMaxLength} characters.");
        
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new DomainException("DOMAIN_CODEITEM_INVALID_CREATED_BY", "CreatedBy is required.");

        return new CodeItem(
            categoryId,
            trimmedCode,
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            sortOrder,
            createdBy);
    }
    
    public void Update(string? description, int sortOrder, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new DomainException("DOMAIN_CODEITEM_INVALID_MODIFIED_BY", "ModifiedBy is required.");

        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        SortOrder = sortOrder;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
    }
    
    public void Deactivate(string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new DomainException("DOMAIN_CODEITEM_INVALID_MODIFIED_BY", "ModifiedBy is required.");

        if (!IsActive) return;

        IsActive = false;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
    }
    
    public void Activate(string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new DomainException("DOMAIN_CODEITEM_INVALID_MODIFIED_BY", "ModifiedBy is required.");

        if (IsActive) return;

        IsActive = true;
        ModifiedAtUtc = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
    }
}