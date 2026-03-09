using TouCart.Models;

namespace TouCart.Data;

public class CategoryRepository : ICategoryRepository
{
    private readonly DatabaseContext _context;

    public CategoryRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        var conn = await _context.GetConnectionAsync();
        return await conn.Table<Category>()
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var conn = await _context.GetConnectionAsync();
        return await conn.Table<Category>()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        var conn = await _context.GetConnectionAsync();
        var existing = await conn.Table<Category>().OrderByDescending(c => c.SortOrder).FirstOrDefaultAsync();
        category.SortOrder = (existing?.SortOrder ?? 0) + 10;
        await conn.InsertAsync(category);
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        var conn = await _context.GetConnectionAsync();
        await conn.UpdateAsync(category);
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var conn = await _context.GetConnectionAsync();
        await conn.DeleteAsync<Category>(id);
    }
}
