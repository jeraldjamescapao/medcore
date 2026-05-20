namespace MedCorVis.Modules.Users.Presentation.Controllers;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedCorVis.Common.Authorization;
using MedCorVis.Common.Controllers;
using MedCorVis.Modules.Users.Application.Abstractions;

[Authorize(Roles = $"{AppRoles.Admin}, {AppRoles.MedicalSecretary}")]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/users")]
public sealed class UsersController : BaseApiController
{
    private readonly IUserDeletionService _userDeletionService;

    public UsersController(IUserDeletionService userDeletionService)
    {
        _userDeletionService = userDeletionService;
    }
    
    [HttpGet("deletion-requests")]
    public async Task<IActionResult> GetPendingDeletionRequestsAsync(CancellationToken ct)
    {
        var result = await _userDeletionService.GetPendingDeletionRequestsAsync(ct);
        return ToActionResult(result);
    }
    
    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> ExecuteDeletionAsync(Guid id, CancellationToken ct)
    {
        var result = await _userDeletionService.ExecuteDeletionAsync(id, ct);
        return result.IsFailure ? 
            ToActionResult(result) : NoContent();
    }
}