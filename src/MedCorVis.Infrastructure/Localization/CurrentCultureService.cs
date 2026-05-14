namespace MedCorVis.Infrastructure.Localization;

using MedCorVis.Common.Localization;
using MedCorVis.Common.Services;

internal sealed class CurrentCultureService : ICurrentCultureService
{
    public string Culture { get; private set; } = SupportedCultures.Default;

    public void SetCulture(string culture) => Culture = culture;
}