namespace MedCore.Common.Caching;

public interface IUserCultureCache
{
    void InvalidateForUser(Guid userId);
}