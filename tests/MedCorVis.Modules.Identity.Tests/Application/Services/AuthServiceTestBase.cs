namespace MedCorVis.Modules.Identity.Tests.Application.Services;

using MedCorVis.Common.Authorization;
using MedCorVis.Common.Caching;
using MedCorVis.Common.Localization;
using MedCorVis.Common.Services;
using MedCorVis.Modules.Identity.Application.Abstractions.Authentication;
using MedCorVis.Modules.Identity.Application.Abstractions.Email;
using MedCorVis.Modules.Identity.Application.Abstractions.Persistence;
using MedCorVis.Modules.Identity.Application.Services;
using MedCorVis.Modules.Identity.Configuration;
using MedCorVis.Modules.Identity.Domain.Tokens;
using MedCorVis.Modules.Identity.Domain.Users;
using MedCorVis.Modules.Identity.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

public abstract class AuthServiceTestBase
{
    protected readonly UserManager<ApplicationUser> UserManager;
    internal readonly IJwtTokenService JwtTokenService;
    internal readonly IRefreshTokenRepository RefreshTokenRepository;
    internal readonly IIdentityEmailService IdentityEmailService;
    internal readonly IIdentityUnitOfWork UnitOfWork;
    protected readonly IDbContextTransaction Transaction;
    protected readonly IAuthService Sut;
    protected readonly ICurrentCultureService CurrentCultureService;
    protected readonly IUserCultureCache UserCultureCache;
    
    protected static readonly JwtSettings DefaultJwtSettings = new()
    {
        SecretKey  = "TesterJamesCapaoLongSecretKeyGoesLikeThis",
        Issuer     = "MedCorVis",
        Audience   = "MedCorVis",
        AccessTokenExpirationInMinutes = 15,
        RefreshTokenExpirationInDays   = 7
    };
    
    protected AuthServiceTestBase()
    {
        UserManager            = MockUserManager.Create();
        JwtTokenService        = Substitute.For<IJwtTokenService>();
        RefreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        IdentityEmailService   = Substitute.For<IIdentityEmailService>();
        UnitOfWork             = Substitute.For<IIdentityUnitOfWork>();
        Transaction            = Substitute.For<IDbContextTransaction>();
        
        CurrentCultureService = Substitute.For<ICurrentCultureService>();
        CurrentCultureService.Culture.Returns(SupportedCultures.Default);
        UserCultureCache = Substitute.For<IUserCultureCache>();
        
        UnitOfWork
            .BeginTransactionAsync(Arg.Any<CancellationToken>())
            .Returns(Transaction);
        
        JwtTokenService.GenerateAccessToken(Arg.Any<ApplicationUser>(), Arg.Any<IList<string>>())
            .Returns("access-token");

        JwtTokenService.GenerateRefreshToken()
            .Returns("raw-refresh-token");
        
        Sut = new AuthService(
            UserManager,
            CurrentCultureService,
            UserCultureCache,
            JwtTokenService,
            RefreshTokenRepository,
            IdentityEmailService,
            UnitOfWork,
            Options.Create(DefaultJwtSettings),
            NullLogger<AuthService>.Instance);
    }
    
    protected static ApplicationUser CreateUser(
        string email     = "jjcapaotest@softwareengineers.ch",
        bool isActive    = true,
        bool emailConfirmed = false)
    {
        var user = ApplicationUser.Create(
            email,
            "Jerald James Capao",
            "Test",
            new DateOnly(1988, 6, 27),
            ApplicationUser.SelfRegisteredActor);

        // EmailConfirmed has no setter — use reflection to set it for testing.
        typeof(ApplicationUser)
            .GetProperty(nameof(ApplicationUser.EmailConfirmed))!
            .SetValue(user, emailConfirmed);

        if (!isActive)
        {
            user.Deactivate(SystemActors.System);
        }

        return user;
    }
}