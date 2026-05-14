namespace MedCorVis.Common.Services;

public interface ICurrentUserService
{
    string UserId { get; }
    bool IsAuthenticated { get; }
}