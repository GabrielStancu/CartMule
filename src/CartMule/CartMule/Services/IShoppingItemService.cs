using CartMule.Models;

namespace CartMule.Services;

public interface IShoppingItemService
{
    /// <summary>
    /// Returns items for a list sorted by: IsBought ASC → Category.SortOrder ASC → SortOrder ASC.
    /// </summary>
    Task<List<ShoppingItem>> GetItemsForListAsync(int listId);
    Task<ShoppingItem> AddItemAsync(int listId, string name, int categoryId, string quantity);
    Task<ShoppingItem> UpdateItemAsync(ShoppingItem item);
    Task DeleteItemAsync(int id);
    /// <summary>Toggles IsBought and updates the list's UpdatedAt timestamp.</summary>
    Task<ShoppingItem> ToggleBoughtAsync(int id);
    /// <summary>Sets all items in a list to IsBought=false and touches the list.</summary>
    Task UncheckAllAsync(int listId);
}
