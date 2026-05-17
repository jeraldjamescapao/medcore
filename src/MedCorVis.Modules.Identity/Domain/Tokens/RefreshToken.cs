namespace MedCorVis.Modules.Identity.Domain.Tokens;

using MedCorVis.Common.Domain;

internal sealed class RefreshToken
{
    #region Constants
    
    public const int TokenMaxLength = 500;
    
    #endregion
    
    #region Properties
    
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid FamilyId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAtUtc;
    public bool IsActive => !IsRevoked && !IsExpired;
    
    #endregion
    
    #region Constructors
    
    private RefreshToken() { }

    private RefreshToken(
        Guid userId, 
        Guid familyId, 
        string token, 
        DateTimeOffset expiresAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        FamilyId = familyId;
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        IsRevoked = false;
    }
    
    #endregion
    
    #region Factory

    public static RefreshToken Create(
        Guid userId, 
        Guid familyId, 
        string token, 
        DateTimeOffset expiresAtUtc)
    {
        DomainGuards.RequireNonEmptyGuid(
            userId, "DOMAIN_TOKEN_INVALID_USER_ID", "UserId cannot be empty.");
        DomainGuards.RequireNonEmptyGuid(
            familyId, "DOMAIN_TOKEN_INVALID_FAMILY_ID", "FamilyId cannot be empty.");
        DomainGuards.RequireNonEmpty(
            token, "DOMAIN_TOKEN_INVALID_TOKEN", "Token cannot be empty.");
        DomainGuards.RequireFutureDate(
            expiresAtUtc, "DOMAIN_TOKEN_INVALID_EXPIRY", "ExpiresAtUtc must be in the future.");
        
        return new RefreshToken(userId, familyId, token, expiresAtUtc);
    }
    
    #endregion
    
    #region Methods
    
    public void Revoke()
    {
        if (IsRevoked) return;
        IsRevoked = true;
    }
    
    public void MarkReplacedBy(Guid newTokenId)
    {
        DomainGuards.RequireNonEmptyGuid(
            newTokenId, "DOMAIN_TOKEN_INVALID_REPLACEMENT_ID", "New token ID cannot be empty.");
        
        ReplacedByTokenId = newTokenId;
    }
    
    #endregion
}