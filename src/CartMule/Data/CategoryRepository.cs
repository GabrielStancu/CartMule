using CartMule.Models;

namespace CartMule.Data;

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
}
