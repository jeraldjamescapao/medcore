namespace MedCore.Modules.CodeItems.Infrastructure.Persistence;

using MedCore.Modules.CodeItems.Domain;
using MedCore.Modules.CodeItems.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;

internal sealed class CodeItemsDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CodeItem> Items => Set<CodeItem>();
    public DbSet<CodeItemTranslation> Translations => Set<CodeItemTranslation>();
    
    public CodeItemsDbContext(DbContextOptions<CodeItemsDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("CodeItems");

        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new CodeItemConfiguration());
        builder.ApplyConfiguration(new CodeItemTranslationConfiguration());
    }
}