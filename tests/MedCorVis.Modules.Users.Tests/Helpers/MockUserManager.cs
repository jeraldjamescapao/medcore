namespace MedCorVis.Modules.Users.Tests.Helpers;

using Microsoft.AspNetCore.Identity;
using NSubstitute;
using MedCorVis.Modules.Identity.Domain.Users;

internal static class MockUserManager
{
    public static UserManager<ApplicationUser> Create()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        return Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
    } 
}