namespace MedCorVis.Modules.Users.Application.Abstractions;

using MedCorVis.Common.Results;
using MedCorVis.Modules.Users.Application.Contracts.Responses;

public interface IUserDeletionService
{
    Task<Result<bool>> RequestDeletionAsync(CancellationToken ct = default);
    Task<Result<bool>> CancelDeletionRequestAsync(CancellationToken ct = default);
    Task<Result<IReadOnlyList<DeletionRequestResponse>>> GetPendingDeletionRequestsAsync(CancellationToken ct = default);
    Task<Result<bool>> ExecuteDeletionAsync(Guid targetUserId, CancellationToken ct = default);
}