namespace MedCorVis.Common.Persistence;

using MedCorVis.Common.Auditing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

/// <summary>
/// Base DbContext for all MedCorVis modules.
/// Automatically applies a global query filter on all entities
/// implementing <see cref="IDeletableEntity"/> to exclude soft-deleted records.
/// Use <c>.IgnoreQueryFilters()</c> on a specific query to bypass this filter.
/// </summary>
public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ApplyDeletableEntityFilter(builder);
    }

    private static void ApplyDeletableEntityFilter(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(IDeletableEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var param = Expression.Parameter(entityType.ClrType, "e");
            var isDeleted = Expression.Property(param, nameof(IDeletableEntity.IsDeleted));
            var notDeleted = Expression.Not(isDeleted);
            var lambda = Expression.Lambda(notDeleted, param);

            builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}