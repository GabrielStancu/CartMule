using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CartMule.Models;
using CartMule.Services;

namespace CartMule.ViewModels;

public class ItemGroup : ObservableCollection<ShoppingItem>
{
    public string Name { get; }
    public bool IsBoughtGroup { get; }

    public ItemGroup(string name, bool isBoughtGroup, IEnumerable<ShoppingItem> items) : base(items)
    {
        Name = name;
        IsBoughtGroup = isBoughtGroup;
    }
}

[QueryProperty(nameof(ListId), "id")]
public partial class ListDetailViewModel : BaseViewModel
{
    private readonly IShoppingListService _listService;
    private readonly IShoppingItemService _itemService;
    private readonly ICategoryService _categoryService;

    public ObservableCollection<ItemGroup> Groups { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEmpty))]
    bool _isEmpty = true;

    public bool IsNotEmpty => !IsEmpty;

    [ObservableProperty]
    bool _hasBoughtItems;

    public int ListId { get; set; }

    public ListDetailViewModel(
        IShoppingListService listService,
        IShoppingItemService itemService,
        ICategoryService categoryService)
    {
        _listService = listService;
        _itemService = itemService;
        _categoryService = categoryService;
    }

    [RelayCommand]
    async Task LoadItemsAsync()
    {
        if (ListId == 0 || IsBusy) return;
        IsBusy = true;
        try
        {
            var list = await _listService.GetListByIdAsync(ListId);
            Title = list?.Name ?? "List";

            var categories = await _categoryService.GetAllCategoriesAsync();
            var catNames = categories.ToDictionary(c => c.Id, c => c.Name);

            var items = await _itemService.GetItemsForListAsync(ListId);
            RebuildGroups(items, catNames);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task ToggleBoughtAsync(ShoppingItem item)
    {
        await _itemService.ToggleBoughtAsync(item.Id);
        await LoadItemsAsync();
    }

    [RelayCommand]
    async Task DeleteItemAsync(ShoppingItem item)
    {
        await _itemService.DeleteItemAsync(item.Id);
        foreach (var group in Groups)
        {
            if (group.Remove(item))
                break;
        }
        var emptyGroups = Groups.Where(g => g.Count == 0).ToList();
        foreach (var g in emptyGroups)
            Groups.Remove(g);
        IsEmpty = Groups.Count == 0;
        HasBoughtItems = Groups.Any(g => g.IsBoughtGroup && g.Count > 0);
    }

    [RelayCommand]
    async Task UncheckAllAsync()
    {
        await _itemService.UncheckAllAsync(ListId);
        await LoadItemsAsync();
    }

    private void RebuildGroups(List<ShoppingItem> items, Dictionary<int, string> catNames)
    {
        Groups.Clear();

        var unbought = items.Where(i => !i.IsBought).ToList();
        var bought = items.Where(i => i.IsBought).ToList();

        // Preserve category sort order from the service's sorted flat list
        foreach (var g in unbought.GroupBy(i => i.CategoryId))
        {
            var name = catNames.GetValueOrDefault(g.Key, "Other");
            Groups.Add(new ItemGroup(name, false, g.ToList()));
        }

        // All bought items land in a single "In Cart" group at the bottom
        if (bought.Count > 0)
            Groups.Add(new ItemGroup("In Cart ✓", true, bought));

        IsEmpty = Groups.Count == 0;
        HasBoughtItems = bought.Count > 0;
    }
}
