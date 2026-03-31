namespace PatientManagementSystem.Modules.Identity.Presentation.Controllers;

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PatientManagementSystem.Common.Controllers;
using PatientManagementSystem.Modules.Identity.Application.Abstractions.Authentication;

[ApiVersion("1")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
}