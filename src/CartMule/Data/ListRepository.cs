using CartMule.Models;

namespace CartMule.Data;

public class ListRepository : IListRepository
{
    private readonly DatabaseContext _context;

    public ListRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<ShoppingList>> GetAllAsync()
    {
        var conn = await _context.GetConnectionAsync();
        return await conn.Table<ShoppingList>()
            .OrderByDescending(l => l.UpdatedAt)
            .ToListAsync();
    }

    public async Task<ShoppingList?> GetByIdAsync(int id)
    {
        var conn = await _context.GetConnectionAsync();
        return await conn.Table<ShoppingList>()
            .Where(l => l.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<ShoppingList> CreateAsync(ShoppingList list)
    {
        var conn = await _context.GetConnectionAsync();
        list.CreatedAt = DateTime.UtcNow;
        list.UpdatedAt = DateTime.UtcNow;
        await conn.InsertAsync(list);
        return list;
    }

    public async Task<ShoppingList> UpdateAsync(ShoppingList list)
    {
        var conn = await _context.GetConnectionAsync();
        await conn.UpdateAsync(list);
        return list;
    }

    public async Task DeleteAsync(int id)
    {
        var conn = await _context.GetConnectionAsync();
        await conn.DeleteAsync<ShoppingList>(id);
    }
}
