namespace MedCorVis.Common.Services;

public interface ICurrentCultureService
{
    string Culture { get; }
    void SetCulture(string culture);
}