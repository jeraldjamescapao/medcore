namespace MedCorVis.Common.Auditing;

public interface IDeletableEntity
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAtUtc { get; }
    string? DeletedBy { get; }
}