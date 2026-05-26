namespace MedCorVis.Modules.Identity.Application.Contracts.Authentication.Requests;

using MedCorVis.Common.Validations;
using System.ComponentModel.DataAnnotations;
using User = MedCorVis.Modules.Identity.Domain.Users.ApplicationUser;

public sealed record RegisterRequest(
    [Required] [MinLength(2)] [MaxLength(100)] string FirstName,
    [Required] [MinLength(2)] [MaxLength(100)] string LastName,
    [Required] [EmailAddress] [MaxLength(User.EmailMaxLength)] string Email, 
    [Required] [MinLength(User.PasswordMinLength)] [MaxLength(User.PasswordMaxLength)] string Password,
    [PastDate] DateOnly BirthDate,
    [MaxLength(User.PreferredCultureMaxLength)] string? Culture = null);