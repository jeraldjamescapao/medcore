namespace MedCorVis.Modules.Identity.Application.Contracts.Authentication.Requests;

using MedCorVis.Common.Validations;
using System.ComponentModel.DataAnnotations;
using User = MedCorVis.Modules.Identity.Domain.Users.ApplicationUser;

public sealed record LoginRequest(
    [Required] [EmailAddress] [MaxLength(User.EmailMaxLength)] string Email,
    [Required] [MinLength(User.PasswordMinLength)] [MaxLength(User.PasswordMaxLength)] [StrongPassword] string Password);