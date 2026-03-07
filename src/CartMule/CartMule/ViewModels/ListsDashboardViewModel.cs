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
    public string UpdatedDisplay =>
        List.UpdatedAt == default
            ? "New"
            : List.UpdatedAt.ToLocalTime().ToString("MMM d");
}

public partial class ListsDashboardViewModel : BaseViewModel
{
    private readonly IShoppingListService _listService;

    public ObservableCollection<ListSummaryItem> Lists { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEmpty))]
    bool _isEmpty = true;

    public bool IsNotEmpty => !IsEmpty;

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
        IsEmpty = false;
        try
        {
            var lists = await _listService.GetAllListsAsync();
            Lists.Clear();
            foreach (var list in lists)
            {
                var count = await _listService.GetItemCountAsync(list.Id);
                Lists.Add(new ListSummaryItem { List = list, ItemCount = count });
            }
            IsEmpty = Lists.Count == 0;
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

    [RelayCommand]
    async Task DeleteListAsync(ListSummaryItem item)
    {
        await _listService.DeleteListAsync(item.List.Id);
        Lists.Remove(item);
        IsEmpty = Lists.Count == 0;
    }

    [RelayCommand]
    async Task OpenListAsync(ListSummaryItem item)
    {
        await Shell.Current.GoToAsync($"listdetail?id={item.List.Id}");
    }
}
