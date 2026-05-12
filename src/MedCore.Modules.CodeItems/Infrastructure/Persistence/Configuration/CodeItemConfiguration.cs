namespace MedCore.Modules.CodeItems.Infrastructure.Persistence.Configuration;

using MedCore.Modules.CodeItems.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class CodeItemConfiguration : IEntityTypeConfiguration<CodeItem>
{
    public void Configure(EntityTypeBuilder<CodeItem> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(CodeItem.CodeMaxLength);

        builder.Property(x => x.Description)
            .HasMaxLength(CodeItem.DescriptionMaxLength);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedAtUtc)
            .HasColumnType("datetimeoffset");

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.CategoryId, x.Code })
            .IsUnique()
            .HasDatabaseName("IX_Items_CategoryId_Code");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Items_IsActive");
    }
}