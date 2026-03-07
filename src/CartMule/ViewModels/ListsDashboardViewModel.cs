using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CartMule.Models;
using CartMule.Services;

namespace CartMule.ViewModels;

public sealed class ListSummaryItem
{
    public ShoppingList List { get; init; } = default!;
    public int ItemCount { get; init; }
    public string CreatedDisplay =>
        List.CreatedAt.ToLocalTime().ToString("MMM d");

    public string UpdatedDisplay =>
        List.UpdatedAt == default
            ? "New"
            : List.UpdatedAt.ToLocalTime().ToString("MMM d");
}

public partial class ListsDashboardViewModel : BaseViewModel
{
    private readonly IShoppingListService _listService;
    private readonly List<ListSummaryItem> _allLists = [];

    public ObservableCollection<ListSummaryItem> FilteredLists { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEmpty))]
    bool _isEmpty = true;

    public bool IsNotEmpty => !IsEmpty;

    [ObservableProperty]
    string _searchQuery = string.Empty;

    partial void OnSearchQueryChanged(string value) => ApplyFilter();

    public ListsDashboardViewModel(IShoppingListService listService)
    {
        _listService = listService;
        Title = "CartMule";
    }

    [RelayCommand]
    async Task LoadListsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var lists = await _listService.GetAllListsAsync();
            _allLists.Clear();
            foreach (var list in lists)
            {
                var count = await _listService.GetItemCountAsync(list.Id);
                _allLists.Add(new ListSummaryItem { List = list, ItemCount = count });
            }
            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task CreateListAsync(string name)
    {
        await _listService.CreateListAsync(name);
        await LoadListsAsync();
    }

    private void ApplyFilter()
    {
        FilteredLists.Clear();
        var q = SearchQuery?.Trim();
        var source = string.IsNullOrEmpty(q)
            ? (IEnumerable<ListSummaryItem>)_allLists
            : _allLists.Where(l => l.List.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
        foreach (var item in source)
            FilteredLists.Add(item);
        IsEmpty = _allLists.Count == 0;
    }

    private ListSummaryItem? _pendingDeleteList;

    [ObservableProperty]
    bool _showDeleteListConfirm;

    [RelayCommand]
    void RequestDeleteList(ListSummaryItem item)
    {
        _pendingDeleteList = item;
        ShowDeleteListConfirm = true;
    }

    [RelayCommand]
    async Task ConfirmDeleteListAsync()
    {
        ShowDeleteListConfirm = false;
        if (_pendingDeleteList is null) return;
        await _listService.DeleteListAsync(_pendingDeleteList.List.Id);
        _allLists.Remove(_pendingDeleteList);
        _pendingDeleteList = null;
        ApplyFilter();
    }

    [RelayCommand]
    void CancelDeleteList()
    {
        ShowDeleteListConfirm = false;
        _pendingDeleteList = null;
    }

    [RelayCommand]
    async Task OpenListAsync(ListSummaryItem item)
    {
        await Shell.Current.GoToAsync($"listdetail?id={item.List.Id}");
    }
}
