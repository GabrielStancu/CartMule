using CartMule.Data;
using CartMule.Models;

namespace CartMule.Services;

public class ShoppingItemService : IShoppingItemService
{
    private readonly IItemRepository _itemRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IShoppingListService _listService;

    public ShoppingItemService(
        IItemRepository itemRepo,
        ICategoryRepository categoryRepo,
        IShoppingListService listService)
    {
        _itemRepo = itemRepo;
        _categoryRepo = categoryRepo;
        _listService = listService;
    }

    public async Task<List<ShoppingItem>> GetItemsForListAsync(int listId)
    {
        var items = await _itemRepo.GetByListIdAsync(listId);
        var categories = await _categoryRepo.GetAllAsync();
        var catOrder = categories.ToDictionary(c => c.Id, c => c.SortOrder);

        return [.. items.OrderBy(i => i.IsBought ? 1 : 0)
                        .ThenBy(i => catOrder.GetValueOrDefault(i.CategoryId, 99))
                        .ThenBy(i => i.SortOrder)];
    }

    public async Task<ShoppingItem> AddItemAsync(int listId, string name, int categoryId, string quantity)
    {
        var item = new ShoppingItem
        {
            ListId = listId,
            Name = name.Trim(),
            CategoryId = categoryId,
            Quantity = quantity.Trim(),
            IsBought = false,
            SortOrder = 0
        };
        var created = await _itemRepo.CreateAsync(item);
        await _listService.TouchListAsync(listId);
        return created;
    }

    public async Task<ShoppingItem> UpdateItemAsync(ShoppingItem item)
    {
        var updated = await _itemRepo.UpdateAsync(item);
        await _listService.TouchListAsync(item.ListId);
        return updated;
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await _itemRepo.GetByIdAsync(id);
        if (item is null) return;
        await _itemRepo.DeleteAsync(id);
        await _listService.TouchListAsync(item.ListId);
    }

    public async Task<ShoppingItem> ToggleBoughtAsync(int id)
    {
        var item = await _itemRepo.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Item {id} not found.");
        item.IsBought = !item.IsBought;
        var updated = await _itemRepo.UpdateAsync(item);
        await _listService.TouchListAsync(item.ListId);
        return updated;
    }

    public async Task UncheckAllAsync(int listId)
    {
        var items = await _itemRepo.GetByListIdAsync(listId);
        foreach (var item in items.Where(i => i.IsBought))
        {
            item.IsBought = false;
            await _itemRepo.UpdateAsync(item);
        }
        await _listService.TouchListAsync(listId);
    }

    public async Task SaveSortOrdersAsync(IEnumerable<ShoppingItem> items)
    {
        await _itemRepo.UpdateManyAsync(items);
    }
}
