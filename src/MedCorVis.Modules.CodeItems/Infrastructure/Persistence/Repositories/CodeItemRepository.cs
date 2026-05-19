namespace MedCorVis.Modules.CodeItems.Infrastructure.Persistence.Repositories;

using MedCorVis.Modules.CodeItems.Application.Abstractions;
using MedCorVis.Modules.CodeItems.Domain;
using Microsoft.EntityFrameworkCore;

internal sealed class CodeItemRepository : ICodeItemRepository
{
    private readonly CodeItemsDbContext _context;

    public CodeItemRepository(CodeItemsDbContext context)
    {
        _context = context;
    }
    
    #region Categories
    
    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync(CancellationToken ct = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);
    }
    
    public async Task<Category?> GetCategoryByIdAsync(long id, CancellationToken ct = default)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
    
    public async Task<Category?> GetCategoryByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Code == code, ct);
    }
    
    public async Task<bool> CategoryCodeExistsAsync(string code, CancellationToken ct = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Code == code, ct);
    }
    
    public async Task AddCategoryAsync(Category category, CancellationToken ct = default)
    {
        await _context.Categories.AddAsync(category, ct);
    }
    
    #endregion
    
    #region Items

    public async Task<IReadOnlyList<CodeItem>> GetTrackedItemsByCategoryIdAsync(
        long categoryId, CancellationToken ct = default)
    {
        return await _context.Items
            .Where(i => i.CategoryId == categoryId)
            .OrderBy(i => i.SortOrder)
            .ToListAsync(ct);       
    }
    
    public async Task<IReadOnlyList<CodeItem>> GetTrackedItemsByCategoryIdAndItemIdsAsync(
        long categoryId, IReadOnlyCollection<long> ids, CancellationToken ct = default)
    {
        return await _context.Items
            .Where(i => 
                i.CategoryId == categoryId && 
                ids.Contains(i.Id))
            .ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<CodeItem>> GetItemsByCategoryIdAsync(
        long categoryId, CancellationToken ct = default)
    {
        return await _context.Items
            .AsNoTracking()
            .Where(i => i.CategoryId == categoryId)
            .OrderBy(i => i.SortOrder)
            .ToListAsync(ct);
    }
    
    public async Task<CodeItem?> GetItemByIdAsync(long id, CancellationToken ct = default)
    {
        return await _context.Items
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }
    
    public async Task<bool> ItemCodeExistsAsync(
        long categoryId, string code, CancellationToken ct = default)
    {
        return await _context.Items
            .AsNoTracking()
            .AnyAsync(i => 
                i.CategoryId == categoryId && 
                i.Code == code, 
                ct);
    }
    
    public async Task<bool> CategoryHasActiveItemsAsync(
        long categoryId, CancellationToken ct = default)
    {
        return await _context.Items
            .AsNoTracking()
            .AnyAsync(i => 
                i.CategoryId == categoryId && 
                i.IsActive, 
                ct);
    }
    
    public async Task AddItemAsync(CodeItem item, CancellationToken ct = default)
    {
        await _context.Items.AddAsync(item, ct);
    }
    
    #endregion
    
    #region Translations
    
    public async Task<IReadOnlyList<CodeItemTranslation>> GetTranslationsByEntityAsync(
        string entityType, long entityId, CancellationToken ct = default)
    {
        return await _context.Translations
            .AsNoTracking()
            .Where(t => 
                t.EntityType == entityType && 
                t.EntityId == entityId)
            .OrderBy(t => t.Culture)
            .ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<CodeItemTranslation>> GetTrackedTranslationsByEntityAsync(
        string entityType, long entityId, CancellationToken ct = default)
    {
        return await _context.Translations
            .Where(t => 
                t.EntityType == entityType && 
                t.EntityId == entityId)
            .OrderBy(t => t.Culture)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CodeItemTranslation>> GetTrackedTranslationsByEntityIdsAsync(
        string entityType, IReadOnlyCollection<long> entityIds, CancellationToken ct = default)
    {
        return await _context.Translations
            .Where(t => 
                t.EntityType == entityType && 
                entityIds.Contains(t.EntityId))
            .OrderBy(t => t.Culture)
            .ToListAsync(ct);       
    }
    
    public async Task<CodeItemTranslation?> GetTranslationAsync(
        string entityType, long entityId, string culture, CancellationToken ct = default)
    {
        return await _context.Translations
            .FirstOrDefaultAsync(t =>
                t.EntityType == entityType &&
                t.EntityId   == entityId   &&
                t.Culture    == culture, 
                ct);
    }
    
    public async Task AddTranslationAsync(CodeItemTranslation translation, CancellationToken ct = default)
    {
        await _context.Translations.AddAsync(translation, ct);
    }
    
    #endregion
    
    #region Consumer
    
    public async Task<(Category? Category, IReadOnlyList<CodeItem> Items)> GetActiveByCategoryCodeAsync(
        string categoryCode, DateOnly today, CancellationToken ct = default)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => 
                c.Code == categoryCode && 
                c.IsActive,
                ct);

        if (category is null)
            return (null, []);

        var items = await _context.Items
            .AsNoTracking()
            .Where(i =>
                i.CategoryId == category.Id &&
                i.IsActive   &&
                (i.ValidFrom == null || i.ValidFrom <= today) &&
                (i.ValidTo   == null || i.ValidTo   >= today))
            .OrderBy(i => i.SortOrder)
            .ToListAsync(ct);

        return (category, items);
    }
    
    public async Task<IReadOnlyDictionary<long, string>> GetItemLabelsByCategoryAsync(
        long categoryId, string culture, DateOnly today, CancellationToken ct = default)
    {
        return await _context.Translations
            .AsNoTracking()
            .Where(t =>
                t.EntityType == CodeItemTranslation.EntityTypeItem &&
                t.Culture    == culture                            &&
                t.IsActive                                         &&
                _context.Items
                    .Where(i =>
                        i.CategoryId == categoryId &&
                        i.IsActive   &&
                        (i.ValidFrom == null || i.ValidFrom <= today) &&
                        (i.ValidTo   == null || i.ValidTo   >= today))
                    .Select(i => i.Id)
                    .Contains(t.EntityId))
            .ToDictionaryAsync(t => t.EntityId, t => t.Label, ct);
    }
    
    #endregion
    
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}