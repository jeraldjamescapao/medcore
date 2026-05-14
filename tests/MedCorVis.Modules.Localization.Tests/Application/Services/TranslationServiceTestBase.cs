namespace MedCorVis.Modules.Localization.Tests.Application.Services;

using MedCorVis.Common.Authorization;
using MedCorVis.Common.Localization;
using MedCorVis.Common.Services;
using MedCorVis.Modules.Localization.Application.Abstractions;
using MedCorVis.Modules.Localization.Application.Services;
using MedCorVis.Modules.Localization.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

public abstract class TranslationServiceTestBase
{
    internal readonly ITranslationRepository Repository;
    protected readonly ILocalizerCache LocalizerCache;
    protected readonly ICurrentUserService CurrentUserService;
    protected readonly ITranslationService Sut;
    
    protected TranslationServiceTestBase()
    {
        Repository         = Substitute.For<ITranslationRepository>();
        LocalizerCache     = Substitute.For<ILocalizerCache>();
        CurrentUserService = Substitute.For<ICurrentUserService>();

        CurrentUserService.UserId.Returns(SystemActors.System);

        Sut = new TranslationService(
            Repository,
            LocalizerCache,
            CurrentUserService,
            NullLogger<TranslationService>.Instance);
    }
    
    internal static Translation CreateTranslation(
        string culture = SupportedCultures.English,
        string key     = "app.test.key",
        string value   = "Test Value",
        bool isActive  = true)
    {
        var translation = Translation.Create(culture, key, value, SystemActors.System);

        if (!isActive)
            translation.Deactivate(SystemActors.System);

        return translation;
    }
}