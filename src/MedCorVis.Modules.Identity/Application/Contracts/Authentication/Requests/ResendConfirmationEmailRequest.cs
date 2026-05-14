namespace MedCorVis.Modules.Identity.Application.Contracts.Authentication.Requests;

using System.ComponentModel.DataAnnotations;

public sealed record ResendConfirmationEmailRequest(
    [Required] [EmailAddress] string Email);