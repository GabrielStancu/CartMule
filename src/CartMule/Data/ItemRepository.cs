using CartMule.Models;

namespace CartMule.Data;

public class ItemRepository : IItemRepository
{
    private readonly DatabaseContext _context;

    public ItemRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<ShoppingItem>> GetByListIdAsync(int listId)
    {
        var conn = await _context.GetConnectionAsync();
        // Fetch all items for this list; sorting is done in the service layer
        return await conn.Table<ShoppingItem>()
            .Where(i => i.ListId == listId)
            .ToListAsync();
    }

    public async Task<ShoppingItem?> GetByIdAsync(int id)
    {
        var conn = await _context.GetConnectionAsync();
        return await conn.Table<ShoppingItem>()
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<ShoppingItem> CreateAsync(ShoppingItem item)
    {
        var conn = await _context.GetConnectionAsync();
        await conn.InsertAsync(item);
        return item;
    }

    public async Task<ShoppingItem> UpdateAsync(ShoppingItem item)
    {
        var conn = await _context.GetConnectionAsync();
        await conn.UpdateAsync(item);
        return item;
    }

    public async Task DeleteAsync(int id)
    {
        var conn = await _context.GetConnectionAsync();
        await conn.DeleteAsync<ShoppingItem>(id);
    }

    public async Task DeleteByListIdAsync(int listId)
    {
        var conn = await _context.GetConnectionAsync();
        await conn.ExecuteAsync("DELETE FROM ShoppingItem WHERE ListId = ?", listId);
    }

    public async Task<int> GetCountByListIdAsync(int listId)
    {
        var conn = await _context.GetConnectionAsync();
        return await conn.Table<ShoppingItem>()
            .Where(i => i.ListId == listId)
            .CountAsync();
    }

    public async Task UpdateManyAsync(IEnumerable<ShoppingItem> items)
    {
        var conn = await _context.GetConnectionAsync();
        await conn.UpdateAllAsync(items);
    }
}
