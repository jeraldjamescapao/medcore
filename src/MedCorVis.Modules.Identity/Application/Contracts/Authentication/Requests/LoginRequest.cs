namespace MedCorVis.Modules.Identity.Application.Contracts.Authentication.Requests;

using MedCorVis.Common.Validations;
using System.ComponentModel.DataAnnotations;

public sealed record LoginRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password);