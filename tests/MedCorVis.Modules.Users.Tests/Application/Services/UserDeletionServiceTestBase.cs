namespace MedCorVis.Modules.Users.Tests.Application.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using MedCorVis.Common.Services;
using MedCorVis.Modules.Identity.Domain.Users;
using MedCorVis.Modules.Users.Application.Abstractions;
using MedCorVis.Modules.Users.Application.Services;
using MedCorVis.Modules.Users.Tests.Helpers;
using NSubstitute;

public abstract class UserDeletionServiceTestBase
{
    protected readonly UserManager<ApplicationUser> UserManager;
    protected readonly ICurrentUserService CurrentUserService;
    protected readonly IUserDeletionService Sut;

    protected static readonly Guid CurrentUserId = Guid.NewGuid();
    protected static readonly Guid TargetUserId  = Guid.NewGuid();
    protected static readonly Guid ActorId       = Guid.NewGuid();
    
    protected UserDeletionServiceTestBase()
    {
        UserManager        = MockUserManager.Create();
        CurrentUserService = Substitute.For<ICurrentUserService>();

        CurrentUserService.UserId.Returns(CurrentUserId.ToString());

        Sut = new UserDeletionService(
            UserManager,
            CurrentUserService,
            NullLogger<UserDeletionService>.Instance);
    }
    
    protected static ApplicationUser CreateUser()
    {
        return ApplicationUser.Create(
            email: "jjcapaotest@softwareengineers.ch",
            firstName: "Jerald James Capao",
            lastName: "Test",
            birthDate: new DateOnly(1988, 6, 27),
            createdBy: ApplicationUser.SelfRegisteredActor);
    }
}