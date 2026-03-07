using CartMule.Models;

namespace CartMule.Services;

public interface IShoppingListService
{
    Task<List<ShoppingList>> GetAllListsAsync();
    Task<ShoppingList?> GetListByIdAsync(int id);
    Task<ShoppingList> CreateListAsync(string name);
    Task<ShoppingList> RenameListAsync(int id, string newName);
    Task DeleteListAsync(int id);
    Task TouchListAsync(int id);
    Task<int> GetItemCountAsync(int listId);
}
