namespace MedCorVis.Modules.Users.Infrastructure.Persistence;

using MedCorVis.Common.Auditing;
using MedCorVis.Common.Persistence;
using MedCorVis.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;

internal sealed class UsersDbContext : BaseDbContext
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Profiles");

        builder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Users_UserId");

            entity.Property(x => x.UserId)
                .IsRequired();

            entity.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(UserProfile.FirstNameMaxLength);

            entity.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(UserProfile.LastNameMaxLength);

            entity.Property(x => x.BirthDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(x => x.DeletedAtUtc)
                .HasColumnType("datetimeoffset");

            entity.Property(x => x.DeletedBy)
                .HasMaxLength(IAuditableEntity.ModifiedByMaxLength);

            entity.Property(x => x.CreatedAtUtc)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            entity.Property(x => x.CreatedBy)
                .IsRequired()
                .HasMaxLength(IAuditableEntity.CreatedByMaxLength);

            entity.Property(x => x.ModifiedAtUtc)
                .HasColumnType("datetimeoffset");

            entity.Property(x => x.ModifiedBy)
                .HasMaxLength(IAuditableEntity.ModifiedByMaxLength);
        });
    }
}