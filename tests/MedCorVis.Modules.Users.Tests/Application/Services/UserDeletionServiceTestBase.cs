namespace MedCorVis.Modules.Users.Tests.Application.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using MedCorVis.Common.Services;
using MedCorVis.Common.UserProfiles;
using MedCorVis.Modules.Identity.Domain.Users;
using MedCorVis.Modules.Users.Application.Abstractions;
using MedCorVis.Modules.Users.Application.Services;
using MedCorVis.Modules.Users.Domain;
using MedCorVis.Modules.Users.Tests.Helpers;
using NSubstitute;

public abstract class UserDeletionServiceTestBase
{
    protected readonly UserManager<ApplicationUser> UserManager;
    protected readonly ICurrentUserService CurrentUserService;
    protected readonly IUserDeletionService Sut;
    
    private readonly IUserProfileService    _userProfileService;
    private readonly IUserProfileRepository _userProfileRepository;

    protected static readonly Guid CurrentUserId = Guid.NewGuid();
    protected static readonly Guid TargetUserId  = Guid.NewGuid();
    protected static readonly Guid ActorId       = Guid.NewGuid();
    
    protected UserDeletionServiceTestBase()
    {
        UserManager        = MockUserManager.Create();
        CurrentUserService = Substitute.For<ICurrentUserService>();
        _userProfileService    = Substitute.For<IUserProfileService>();
        _userProfileRepository = Substitute.For<IUserProfileRepository>();

        CurrentUserService.UserId.Returns(CurrentUserId.ToString());

        Sut = new UserDeletionService(
            UserManager,
            CurrentUserService,
            _userProfileService,
            _userProfileRepository,
            NullLogger<UserDeletionService>.Instance);
    }
    
    protected void SetupAnonymise(Guid userId)
    {
        _userProfileService
            .AnonymiseProfileAsync(userId, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
    }
    
    protected void SetupProfiles(IReadOnlyList<UserProfile> profiles)
    {
        _userProfileRepository
            .GetByUserIdsAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(profiles);
    }
    
    protected static ApplicationUser CreateUser(string? email = null)
    {
        return ApplicationUser.Create(
            email: email ?? "jjcapaotest@softwareengineers.ch",
            createdBy: ApplicationUser.SelfRegisteredActor);
    }
    
    protected static UserProfile CreateProfile(
        Guid userId,
        string firstName = "Jerald James Capao",
        string lastName  = "Test")
    {
        return UserProfile.Create(
            userId:    userId,
            firstName: firstName,
            lastName:  lastName,
            birthDate: new DateOnly(1988, 6, 27),
            createdBy: ApplicationUser.SelfRegisteredActor);
    }
    
    protected async Task VerifyAnonymiseCalledOnce(Guid userId)
    {
        await _userProfileService
            .Received(1)
            .AnonymiseProfileAsync(userId, Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}