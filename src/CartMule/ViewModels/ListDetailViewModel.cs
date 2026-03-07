using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CartMule.Models;
using CartMule.Services;

namespace CartMule.ViewModels;

/// <summary>Wraps a ShoppingItem and adds observable drag-state for animation feedback.</summary>
public partial class ShoppingItemViewModel : ObservableObject
{
    public ShoppingItem Source { get; }

    [ObservableProperty]
    bool _isDragging;

    public ShoppingItemViewModel(ShoppingItem source) => Source = source;

    // Forward properties so XAML bindings need no changes
    public int    Id         => Source.Id;
    public string Name       => Source.Name;
    public string Quantity   => Source.Quantity;
    public bool   IsBought   => Source.IsBought;
    public int    ListId     => Source.ListId;
    public int    CategoryId { get => Source.CategoryId; set => Source.CategoryId = value; }
    public int    SortOrder  { get => Source.SortOrder;  set => Source.SortOrder  = value; }
}

public class ItemGroup : ObservableCollection<ShoppingItemViewModel>
{
    public string Name        { get; }
    public bool   IsBoughtGroup { get; }

    public ItemGroup(string name, bool isBoughtGroup, IEnumerable<ShoppingItemViewModel> items)
        : base(items)
    {
        Name         = name;
        IsBoughtGroup = isBoughtGroup;
    }
}

[QueryProperty(nameof(ListId), "id")]
public partial class ListDetailViewModel : BaseViewModel
{
    private readonly IShoppingListService _listService;
    private readonly IShoppingItemService _itemService;
    private readonly ICategoryService     _categoryService;

    public ObservableCollection<ItemGroup> Groups { get; } = new();

    private List<ShoppingItemViewModel> _allItemVms = new();
    private Dictionary<int, string>     _cachedCatNames = new();
    private ShoppingItemViewModel?      _draggedVm;
    private ShoppingItemViewModel?      _pendingDeleteVm;

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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasShops))]
    string _shops = string.Empty;

    public bool HasShops => !string.IsNullOrEmpty(Shops);

    [ObservableProperty]
    bool _showDeleteItemConfirm;

    partial void OnSearchQueryChanged(string value) => ApplyItemFilter();

    public int ListId { get; set; }

    public ListDetailViewModel(
        IShoppingListService listService,
        IShoppingItemService itemService,
        ICategoryService categoryService)
    {
        _listService     = listService;
        _itemService     = itemService;
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
            Title          = list?.Name ?? "List";
            Shops          = list?.Shops ?? string.Empty;
            UpdatedDisplay = list is not null
                ? $"Updated {list.UpdatedAt.ToLocalTime():MMM d, H:mm}"
                : string.Empty;

            var categories  = await _categoryService.GetAllCategoriesAsync();
            _cachedCatNames = categories.ToDictionary(c => c.Id, c => c.Name);
            var items       = await _itemService.GetItemsForListAsync(ListId);

            if (items.Count > 1 && items.All(i => i.SortOrder == 0))
            {
                for (int i = 0; i < items.Count; i++)
                    items[i].SortOrder = i + 1;
                await _itemService.SaveSortOrdersAsync(items);
            }

            _allItemVms = items.Select(i => new ShoppingItemViewModel(i)).ToList();
            ApplyItemFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task ToggleBoughtAsync(ShoppingItemViewModel vm)
    {
        await _itemService.ToggleBoughtAsync(vm.Id);
        await LoadItemsAsync();
    }

    // ── Delete item with confirmation modal ──────────────────────────────────

    [RelayCommand]
    void RequestDeleteItem(ShoppingItemViewModel vm)
    {
        _pendingDeleteVm      = vm;
        ShowDeleteItemConfirm = true;
    }

    [RelayCommand]
    async Task ConfirmDeleteItemAsync()
    {
        ShowDeleteItemConfirm = false;
        if (_pendingDeleteVm is null) return;
        await _itemService.DeleteItemAsync(_pendingDeleteVm.Id);
        _allItemVms.Remove(_pendingDeleteVm);
        _pendingDeleteVm = null;
        ApplyItemFilter();
    }

    [RelayCommand]
    void CancelDeleteItem()
    {
        ShowDeleteItemConfirm = false;
        _pendingDeleteVm      = null;
    }

    [RelayCommand]
    async Task UncheckAllAsync()
    {
        await _itemService.UncheckAllAsync(ListId);
        await LoadItemsAsync();
    }

    [RelayCommand]
    static async Task EditItemAsync(ShoppingItemViewModel vm) =>
        await Shell.Current.GoToAsync($"additem?listId={vm.ListId}&itemId={vm.Id}");

    // ── Drag and drop reorder ────────────────────────────────────────────────

    [RelayCommand]
    void ItemDragStarted(ShoppingItemViewModel vm)
    {
        if (_draggedVm is not null) _draggedVm.IsDragging = false;
        _draggedVm = vm;
        vm.IsDragging = true;
    }

    [RelayCommand]
    void ItemDragCompleted(ShoppingItemViewModel vm)
    {
        if (_draggedVm is not null) _draggedVm.IsDragging = false;
        _draggedVm = null;
    }

    [RelayCommand]
    async Task ItemDropped(ShoppingItemViewModel target)
    {
        if (_draggedVm is null || ReferenceEquals(_draggedVm, target))
        {
            if (_draggedVm is not null) _draggedVm.IsDragging = false;
            _draggedVm = null;
            return;
        }

        _draggedVm.IsDragging = false;

        // Cross-category move: inherit target's category
        _draggedVm.Source.CategoryId = target.Source.CategoryId;

        var ordered = _allItemVms.OrderBy(v => v.SortOrder).ToList();
        ordered.Remove(_draggedVm);
        var idx = ordered.IndexOf(target);
        if (idx < 0) idx = ordered.Count;
        ordered.Insert(idx, _draggedVm);

        for (int i = 0; i < ordered.Count; i++)
            ordered[i].SortOrder = i + 1;

        await _itemService.SaveSortOrdersAsync(ordered.Select(v => v.Source));

        var items   = await _itemService.GetItemsForListAsync(ListId);
        _allItemVms = items.Select(i => new ShoppingItemViewModel(i)).ToList();
        _draggedVm  = null;
        ApplyItemFilter();
    }

    private void ApplyItemFilter()
    {
        var q = SearchQuery?.Trim();
        var filtered = string.IsNullOrEmpty(q)
            ? _allItemVms
            : _allItemVms
                .Where(v => v.Source.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();
        RebuildGroups(filtered, _cachedCatNames);
    }

    private void RebuildGroups(List<ShoppingItemViewModel> items, Dictionary<int, string> catNames)
    {
        Groups.Clear();

        var unbought = items.Where(v => !v.Source.IsBought).ToList();
        var bought   = items.Where(v =>  v.Source.IsBought).ToList();

        foreach (var g in unbought.GroupBy(v => v.Source.CategoryId))
        {
            var name = catNames.GetValueOrDefault(g.Key, "Other");
            Groups.Add(new ItemGroup(name, false, g.ToList()));
        }

        if (bought.Count > 0)
            Groups.Add(new ItemGroup("In Cart ✓", true, bought));

        IsEmpty       = Groups.Count == 0;
        HasBoughtItems = bought.Count > 0;
    }
}
