namespace PatientManagementSystem.Modules.Identity.Application.Contracts.Authentication;

using PatientManagementSystem.Common.Validations;
using System.ComponentModel.DataAnnotations;

public sealed record LoginRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password);