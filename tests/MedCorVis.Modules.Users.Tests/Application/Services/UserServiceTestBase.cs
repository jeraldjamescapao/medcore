namespace MedCorVis.Modules.Users.Tests.Application.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using MedCorVis.Common.Caching;
using MedCorVis.Modules.Identity.Domain.Users;
using MedCorVis.Modules.Users.Application.Abstractions;
using MedCorVis.Modules.Users.Application.Services;
using MedCorVis.Modules.Users.Domain;
using MedCorVis.Modules.Users.Tests.Helpers;
using NSubstitute;

public abstract class UserServiceTestBase
{
    protected readonly UserManager<ApplicationUser> UserManager;
    protected readonly IUserCultureCache UserCultureCache;
    protected readonly IUserService Sut;
    
    private readonly IUserProfileRepository _userProfileRepository;

    protected UserServiceTestBase()
    {
        UserManager = MockUserManager.Create();
        UserCultureCache = Substitute.For<IUserCultureCache>();
        _userProfileRepository = Substitute.For<IUserProfileRepository>();
        
        Sut = new UserService(
            UserManager, 
            UserCultureCache,
            _userProfileRepository,
            NullLogger<UserService>.Instance);
    }
    
    protected void SetupProfile(Guid userId, UserProfile? profile)
    {
        _userProfileRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(profile);
    }

    protected static ApplicationUser CreateUser()
    {
        return ApplicationUser.Create(
            email: "jjcapaotest@softwareengineers.ch",
            createdBy: ApplicationUser.SelfRegisteredActor);
    }
    
    protected static UserProfile CreateProfile(Guid userId)
    {
        return UserProfile.Create(
            userId,
            firstName:  "Jerald James Capao",
            lastName:   "Test",
            birthDate:  new DateOnly(1988, 6, 27),
            createdBy:  ApplicationUser.SelfRegisteredActor);
    }
}