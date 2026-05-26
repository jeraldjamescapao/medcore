namespace MedCorVis.Modules.Users.Tests.Application.Services.UserServiceTests;

using FluentAssertions;
using MedCorVis.Common.Results;
using MedCorVis.Modules.Identity.Domain.Users;
using MedCorVis.Modules.Users.Application.Contracts.Requests;
using MedCorVis.Modules.Users.Domain;
using NSubstitute;
using Xunit;

public sealed class UpdateProfileTests : UserServiceTestBase
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static readonly UpdateProfileRequest ValidRequest = new(
        FirstName: "James Capao",
        LastName:  "Test Update",
        BirthDate: new DateOnly(1965, 10, 10));

    [Fact]
    public async Task UpdateProfileAsync_UserNotFound_ReturnsNotFound()
    {
        UserManager
            .FindByIdAsync(UserId.ToString())
            .Returns((ApplicationUser?)null);

        var result = await Sut.UpdateProfileAsync(UserId, ValidRequest);

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.NotFound);
        result.Error!.Code.Should().Be("USERS_USER_NOT_FOUND");
    }

    [Fact]
    public async Task UpdateProfileAsync_ProfileNotFound_ReturnsNotFound()
    {
        var user = CreateUser();

        UserManager
            .FindByIdAsync(UserId.ToString())
            .Returns(user);

        SetupProfile(UserId, null);

        var result = await Sut.UpdateProfileAsync(UserId, ValidRequest);

        result.IsFailure.Should().BeTrue();
        result.ErrorType.Should().Be(ResultErrorType.NotFound);
        result.Error!.Code.Should().Be("USERS_USER_NOT_FOUND");
    }

    [Fact]
    public async Task UpdateProfileAsync_ValidRequest_ReturnsUpdatedProfile()
    {
        var user    = CreateUser();
        var profile = CreateProfile(user.Id);

        UserManager
            .FindByIdAsync(UserId.ToString())
            .Returns(user);

        SetupProfile(UserId, profile);

        var result = await Sut.UpdateProfileAsync(UserId, ValidRequest);

        result.IsSuccess.Should().BeTrue();
        result.Value!.FirstName.Should().Be("James Capao");
        result.Value.LastName.Should().Be("Test Update");
        result.Value.FullName.Should().Be("James Capao Test Update");
        result.Value.BirthDate.Should().Be(new DateOnly(1965, 10, 10));
    }
}