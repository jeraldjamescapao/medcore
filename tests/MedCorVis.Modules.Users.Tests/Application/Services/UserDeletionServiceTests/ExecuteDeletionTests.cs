namespace MedCorVis.Modules.Users.Tests.Application.Services.UserDeletionServiceTests;

using FluentAssertions;
using MedCorVis.Common.Results;
using MedCorVis.Modules.Identity.Domain.Users;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

public sealed class ExecuteDeletionTests : UserDeletionServiceTestBase
{
    [Fact]
    public async Task ExecuteDeletionAsync_UserNotFound_ReturnsNotFound()
    {
        UserManager
            .FindByIdAsync(TargetUserId.ToString())
            .Returns((ApplicationUser?)null);

        var result = await Sut.ExecuteDeletionAsync(TargetUserId);

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.NotFound);
        result.Error!.Code.Should().Be("USERS_USER_NOT_FOUND");
    }
    
    [Fact]
    public async Task ExecuteDeletionAsync_UserAlreadyDeleted_ReturnsUnprocessableEntity()
    {
        var user = CreateUser();
        user.Delete(ActorId.ToString());

        UserManager
            .FindByIdAsync(TargetUserId.ToString())
            .Returns(user);

        var result = await Sut.ExecuteDeletionAsync(TargetUserId);

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.UnprocessableEntity);
        result.Error!.Code.Should().Be("USERS_USER_ALREADY_DELETED");
    }
    
    [Fact]
    public async Task ExecuteDeletionAsync_IdentityUpdateFails_ReturnsInternal()
    {
        var user = CreateUser();

        UserManager
            .FindByIdAsync(TargetUserId.ToString())
            .Returns(user);

        UserManager
            .UpdateAsync(user)
            .Returns(IdentityResult.Failed(new IdentityError
            {
                Code        = "UpdateError",
                Description = "Update failed."
            }));

        var result = await Sut.ExecuteDeletionAsync(TargetUserId);

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.Internal);
        result.Error!.Code.Should().Be("USERS_DELETION_FAILED");
    }
    
    [Fact]
    public async Task ExecuteDeletionAsync_ValidRequest_SetsIsDeletedAndAnonymisesIdentityPii()
    {
        var user = CreateUser();

        UserManager
            .FindByIdAsync(TargetUserId.ToString())
            .Returns(user);

        UserManager
            .UpdateAsync(user)
            .Returns(IdentityResult.Success);

        SetupAnonymise(TargetUserId);

        var result = await Sut.ExecuteDeletionAsync(TargetUserId);

        result.IsSuccess.Should().BeTrue();
        user.IsDeleted.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        user.DeletedAtUtc.Should().NotBeNull();
        user.Email.Should().StartWith("deleted_");
        user.PhoneNumber.Should().BeNull();
        await VerifyAnonymiseCalledOnce(TargetUserId);
    }
}