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

    private List<ShoppingItem> _allItems = [];
    private Dictionary<int, string> _cachedCatNames = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEmpty))]
    bool _isEmpty = true;

    public bool IsNotEmpty => !IsEmpty;

    [ObservableProperty]
    bool _hasBoughtItems;

    [ObservableProperty]
    string _searchQuery = string.Empty;

    [ObservableProperty]
    string _updatedDisplay = string.Empty;

    partial void OnSearchQueryChanged(string value) => ApplyItemFilter();

    private ShoppingItem? _draggedItem;

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
            UpdatedDisplay = list is not null
                ? $"Updated {list.UpdatedAt.ToLocalTime():MMM d, H:mm}"
                : string.Empty;

            var categories = await _categoryService.GetAllCategoriesAsync();
            _cachedCatNames = categories.ToDictionary(c => c.Id, c => c.Name);
            _allItems = await _itemService.GetItemsForListAsync(ListId);

            // Initialise SortOrder on first load if none have been set yet
            if (_allItems.Count > 1 && _allItems.All(i => i.SortOrder == 0))
            {
                for (int i = 0; i < _allItems.Count; i++)
                    _allItems[i].SortOrder = i + 1;
                await _itemService.SaveSortOrdersAsync(_allItems);
            }

            ApplyItemFilter();
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
        _allItems.Remove(item);
        ApplyItemFilter();
    }

    [RelayCommand]
    void ItemDragStarted(ShoppingItem item) => _draggedItem = item;

    [RelayCommand]
    void ItemDragCompleted(ShoppingItem item) => _draggedItem = null;

    [RelayCommand]
    async Task ItemDropped(ShoppingItem target)
    {
        if (_draggedItem is null || ReferenceEquals(_draggedItem, target))
        {
            _draggedItem = null;
            return;
        }

        // Change category if dropped into a different group
        _draggedItem.CategoryId = target.CategoryId;

        // Rebuild order: remove dragged, insert before target
        var ordered = _allItems.OrderBy(i => i.SortOrder).ToList();
        ordered.Remove(_draggedItem);
        var targetIdx = ordered.IndexOf(target);
        if (targetIdx < 0) targetIdx = ordered.Count;
        ordered.Insert(targetIdx, _draggedItem);

        for (int i = 0; i < ordered.Count; i++)
            ordered[i].SortOrder = i + 1;

        await _itemService.SaveSortOrdersAsync(ordered);

        // Reload from service so category-ordering is applied correctly
        _allItems = await _itemService.GetItemsForListAsync(ListId);
        _draggedItem = null;
        ApplyItemFilter();
    }

    [RelayCommand]
    async Task UncheckAllAsync()
    {
        await _itemService.UncheckAllAsync(ListId);
        await LoadItemsAsync();
    }

    [RelayCommand]
    static async Task EditItemAsync(ShoppingItem item) =>
        await Shell.Current.GoToAsync($"additem?listId={item.ListId}&itemId={item.Id}");

    public async Task RenameListAsync(string newName)
    {
        await _listService.RenameListAsync(ListId, newName);
        Title = newName;
    }

    private void ApplyItemFilter()
    {
        var q = SearchQuery?.Trim();
        var filtered = string.IsNullOrEmpty(q)
            ? _allItems
            : _allItems.Where(i => i.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        RebuildGroups(filtered, _cachedCatNames);
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
