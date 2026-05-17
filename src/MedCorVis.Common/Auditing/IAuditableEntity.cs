namespace MedCorVis.Common.Auditing;

using System;

public interface IAuditableEntity
{
    public const int CreatedByMaxLength = 100;
    public const int ModifiedByMaxLength = 100;
    
    DateTimeOffset CreatedAtUtc { get; }
    string CreatedBy { get; }
    DateTimeOffset? ModifiedAtUtc { get; }
    string? ModifiedBy { get; }
}