namespace MedCorVis.Modules.Users.Tests.Application.Services.UserDeletionServiceTests;

using FluentAssertions;
using MedCorVis.Common.Results;
using MedCorVis.Modules.Identity.Domain.Users;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

public sealed class CancelDeletionRequestTests : UserDeletionServiceTestBase
{
    [Fact]
    public async Task CancelDeletionRequestAsync_UserNotFound_ReturnsNotFound()
    {
        UserManager
            .FindByIdAsync(CurrentUserId.ToString())
            .Returns((ApplicationUser?)null);

        var result = await Sut.CancelDeletionRequestAsync();

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.NotFound);
        result.Error!.Code.Should().Be("USERS_USER_NOT_FOUND");
    }
    
    [Fact]
    public async Task CancelDeletionRequestAsync_NoPendingRequest_ReturnsUnprocessableEntity()
    {
        var user = CreateUser();

        UserManager
            .FindByIdAsync(CurrentUserId.ToString())
            .Returns(user);

        var result = await Sut.CancelDeletionRequestAsync();

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.UnprocessableEntity);
        result.Error!.Code.Should().Be("USERS_NO_DELETION_REQUEST_PENDING");
    }
    
    [Fact]
    public async Task CancelDeletionRequestAsync_IdentityUpdateFails_ReturnsInternal()
    {
        var user = CreateUser();
        user.RequestDeletion();

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

        var result = await Sut.CancelDeletionRequestAsync();

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.Internal);
        result.Error!.Code.Should().Be("USERS_DELETION_FAILED");
    }
    
    [Fact]
    public async Task CancelDeletionRequestAsync_PendingRequest_ClearsDeletionRequestedAtUtc()
    {
        var user = CreateUser();
        user.RequestDeletion();

        UserManager
            .FindByIdAsync(CurrentUserId.ToString())
            .Returns(user);

        UserManager
            .UpdateAsync(user)
            .Returns(IdentityResult.Success);

        var result = await Sut.CancelDeletionRequestAsync();

        result.IsSuccess.Should().BeTrue();
        user.DeletionRequestedAtUtc.Should().BeNull();
        await UserManager
            .Received(1)
            .UpdateAsync(user);
    }
}