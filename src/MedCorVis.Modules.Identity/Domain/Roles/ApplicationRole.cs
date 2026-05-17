namespace MedCorVis.Modules.Identity.Domain.Roles;

using Microsoft.AspNetCore.Identity;

internal sealed class ApplicationRole : IdentityRole<Guid>
{
    public const int MaxDescriptionLength = 250;
    
    public string Description { get; private set; } = null!;

    private ApplicationRole() { }
    
    public ApplicationRole(string roleName, string description) 
        : base(roleName)
    {
        Id = Guid.NewGuid();
        Description = description;
    }
}