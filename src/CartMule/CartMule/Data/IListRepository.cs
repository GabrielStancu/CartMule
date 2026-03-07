using CartMule.Models;

namespace CartMule.Data;

public interface IListRepository
{
    Task<List<ShoppingList>> GetAllAsync();
    Task<ShoppingList?> GetByIdAsync(int id);
    Task<ShoppingList> CreateAsync(ShoppingList list);
    Task<ShoppingList> UpdateAsync(ShoppingList list);
    Task DeleteAsync(int id);
}
