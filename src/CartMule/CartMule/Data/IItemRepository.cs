using CartMule.Models;

namespace CartMule.Data;

public interface IItemRepository
{
    Task<List<ShoppingItem>> GetByListIdAsync(int listId);
    Task<ShoppingItem?> GetByIdAsync(int id);
    Task<ShoppingItem> CreateAsync(ShoppingItem item);
    Task<ShoppingItem> UpdateAsync(ShoppingItem item);
    Task DeleteAsync(int id);
    Task DeleteByListIdAsync(int listId);
    Task<int> GetCountByListIdAsync(int listId);
}
