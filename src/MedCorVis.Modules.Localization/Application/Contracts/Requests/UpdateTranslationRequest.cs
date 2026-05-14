using MedCorVis.Modules.Localization.Domain;

namespace MedCorVis.Modules.Localization.Application.Contracts.Requests;

using Localization.Domain;
using System.ComponentModel.DataAnnotations;

public sealed record UpdateTranslationRequest(
    [Required] string Value,
    [MaxLength(Translation.DescriptionMaxLength)] string? Description);