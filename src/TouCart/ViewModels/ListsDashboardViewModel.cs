using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TouCart.Models;
using TouCart.Services;

namespace TouCart.ViewModels;

public sealed class ListSummaryItem
{
    public ShoppingList List         { get; init; } = default!;
    public int          ItemCount    { get; init; }
    public string       ItemCountDisplay { get; init; } = string.Empty;
    public string       CreatedDisplay   { get; init; } = string.Empty;
    public string       UpdatedDisplay   { get; init; } = string.Empty;
}

public partial class ListsDashboardViewModel : BaseViewModel
{
    private readonly IShoppingListService _listService;
    private readonly ILocalizationService _loc;
    private readonly List<ListSummaryItem> _allLists = [];

    public ILocalizationService Loc => _loc;

    public ObservableCollection<ListSummaryItem> FilteredLists { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEmpty))]
    bool _isEmpty = true;

    public bool IsNotEmpty => !IsEmpty;

    [ObservableProperty]
    string _searchQuery = string.Empty;

    [ObservableProperty]
    bool _showLanguagePicker;

    partial void OnSearchQueryChanged(string value) => ApplyFilter();

    public ListsDashboardViewModel(IShoppingListService listService, ILocalizationService loc)
    {
        _listService = listService;
        _loc = loc;
        Title = "TouCart";
        _loc.PropertyChanged += OnLocChanged;
    }

    private async void OnLocChanged(object? sender, PropertyChangedEventArgs e)
    {
        await LoadListsAsync();
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
                _allLists.Add(new ListSummaryItem
                {
                    List             = list,
                    ItemCount        = count,
                    ItemCountDisplay = _loc.FormatItemCount(count),
                    CreatedDisplay   = list.CreatedAt.ToLocalTime().ToString("MMM d"),
                    UpdatedDisplay   = list.UpdatedAt == default
                                       ? _loc.New
                                       : list.UpdatedAt.ToLocalTime().ToString("MMM d")
                });
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

    [RelayCommand]
    void OpenLanguagePicker() => ShowLanguagePicker = true;

    [RelayCommand]
    void SelectLanguage(string code)
    {
        _loc.SetLanguage(code);
        ShowLanguagePicker = false;
    }

    [RelayCommand]
    void CloseLanguagePicker() => ShowLanguagePicker = false;
}
