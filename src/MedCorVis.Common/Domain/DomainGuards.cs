namespace MedCorVis.Common.Domain;

using MedCorVis.Common.Exceptions;
using MedCorVis.Common.Localization;

public static class DomainGuards
{
    public static string RequireNonEmpty(
        string value, string code, string message, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(code, message);
    
        var trimmed = value.Trim();
    
        if (maxLength.HasValue && trimmed.Length > maxLength.Value)
            throw new DomainException(code, $"Value cannot exceed {maxLength.Value} characters.");
    
        return trimmed;
    }
    
    public static Guid RequireNonEmptyGuid(Guid value, string code, string message)
    {
        return value == Guid.Empty ? 
            throw new DomainException(code, message) : value;
    }

    public static void RequirePastOrPresentDate(DateOnly date, string code, string message)
    {
        if (date > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException(code, message);
    }
    
    public static void RequireFutureDate(DateTimeOffset value, string code, string message)
    {
        if (value <= DateTimeOffset.UtcNow)
            throw new DomainException(code, message);
    }
    
    public static string? RequireValidCulture(string? culture, string code, string message)
    {
        if (culture is null) return null;
    
        var trimmed = culture.Trim();
    
        return !SupportedCultures.All.Contains(trimmed) ? 
            throw new DomainException(code, message) : trimmed;
    }
}