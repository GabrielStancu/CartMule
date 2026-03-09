using TouCart.Data;
using TouCart.Models;

namespace TouCart.Services;

public class ShoppingListService : IShoppingListService
{
    private readonly IListRepository _listRepo;
    private readonly IItemRepository _itemRepo;

    public ShoppingListService(IListRepository listRepo, IItemRepository itemRepo)
    {
        _listRepo = listRepo;
        _itemRepo = itemRepo;
    }

    public Task<List<ShoppingList>> GetAllListsAsync() =>
        _listRepo.GetAllAsync();

    public Task<ShoppingList?> GetListByIdAsync(int id) =>
        _listRepo.GetByIdAsync(id);

    public Task<ShoppingList> CreateListAsync(string name, string shops = "")
    {
        var list = new ShoppingList { Name = name.Trim(), Shops = shops.Trim() };
        return _listRepo.CreateAsync(list);
    }

    public async Task<ShoppingList> UpdateListAsync(int id, string name, string shops)
    {
        var list = await _listRepo.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"List {id} not found.");
        list.Name = name.Trim();
        list.Shops = shops.Trim();
        list.UpdatedAt = DateTime.UtcNow;
        return await _listRepo.UpdateAsync(list);
    }

    public async Task<ShoppingList> RenameListAsync(int id, string newName)
    {
        var list = await _listRepo.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"List {id} not found.");
        list.Name = newName.Trim();
        list.UpdatedAt = DateTime.UtcNow;
        return await _listRepo.UpdateAsync(list);
    }

    public async Task DeleteListAsync(int id)
    {
        await _itemRepo.DeleteByListIdAsync(id);
        await _listRepo.DeleteAsync(id);
    }

    public async Task TouchListAsync(int id)
    {
        var list = await _listRepo.GetByIdAsync(id);
        if (list is null) return;
        list.UpdatedAt = DateTime.UtcNow;
        await _listRepo.UpdateAsync(list);
    }

    public Task<int> GetItemCountAsync(int listId) =>
        _itemRepo.GetCountByListIdAsync(listId);
}
