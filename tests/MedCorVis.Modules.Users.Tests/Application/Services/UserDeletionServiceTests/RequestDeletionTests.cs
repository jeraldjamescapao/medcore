namespace MedCorVis.Modules.Users.Tests.Application.Services.UserDeletionServiceTests;

using FluentAssertions;
using MedCorVis.Common.Results;
using MedCorVis.Modules.Identity.Domain.Users;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

public sealed class RequestDeletionTests : UserDeletionServiceTestBase
{
    [Fact]
    public async Task RequestDeletionAsync_UserNotFound_ReturnsNotFound()
    {
        UserManager
            .FindByIdAsync(CurrentUserId.ToString())
            .Returns((ApplicationUser?)null);

        var result = await Sut.RequestDeletionAsync();

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.NotFound);
        result.Error!.Code.Should().Be("USERS_USER_NOT_FOUND");
    }
    
    [Fact]
    public async Task RequestDeletionAsync_AlreadyPending_ReturnsConflict()
    {
        var user = CreateUser();
        user.RequestDeletion();

        UserManager
            .FindByIdAsync(CurrentUserId.ToString())
            .Returns(user);

        var result = await Sut.RequestDeletionAsync();

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.Conflict);
        result.Error!.Code.Should().Be("USERS_DELETION_REQUEST_ALREADY_PENDING");
    }
    
    [Fact]
    public async Task RequestDeletionAsync_IdentityUpdateFails_ReturnsInternal()
    {
        var user = CreateUser();

        UserManager
            .FindByIdAsync(CurrentUserId.ToString())
            .Returns(user);

        UserManager
            .UpdateAsync(user)
            .Returns(IdentityResult.Failed(new IdentityError
            {
                Code        = "UpdateError",
                Description = "Update failed."
            }));

        var result = await Sut.RequestDeletionAsync();

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.Internal);
        result.Error!.Code.Should().Be("USERS_DELETION_FAILED");
    }
    
    [Fact]
    public async Task RequestDeletionAsync_ValidRequest_SetsDeletionRequestedAtUtc()
    {
        var user = CreateUser();

        UserManager
            .FindByIdAsync(CurrentUserId.ToString())
            .Returns(user);

        UserManager
            .UpdateAsync(user)
            .Returns(IdentityResult.Success);

        var result = await Sut.RequestDeletionAsync();

        result.IsSuccess.Should().BeTrue();
        user.DeletionRequestedAtUtc.Should().NotBeNull();
        await UserManager
            .Received(1)
            .UpdateAsync(user);
    }
}