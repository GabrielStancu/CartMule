using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CartMule.Models;
using CartMule.Services;

namespace CartMule.ViewModels;

[QueryProperty(nameof(ListId), "listId")]
[QueryProperty(nameof(ItemId), "itemId")]
public partial class AddItemViewModel : BaseViewModel
{
    private readonly IShoppingItemService _itemService;
    private readonly ICategoryService _categoryService;

    public ObservableCollection<Category> Categories { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    string _name = string.Empty;

    [ObservableProperty]
    string _quantity = string.Empty;

    [ObservableProperty]
    string _subtitle = string.Empty;

    [ObservableProperty]
    Category? _selectedCategory;

    public int ListId { get; set; }
    public int ItemId { get; set; }

    public bool IsEditMode => ItemId > 0;

    public AddItemViewModel(IShoppingItemService itemService, ICategoryService categoryService)
    {
        _itemService = itemService;
        _categoryService = categoryService;
    }

    [RelayCommand]
    async Task InitialiseAsync()
    {
        IsBusy = true;
        try
        {
            var cats = await _categoryService.GetAllCategoriesAsync();
            Categories.Clear();
            foreach (var c in cats)
                Categories.Add(c);

            if (IsEditMode)
            {
                Title    = "Edit Item";
                Subtitle = "Update item details";
                // Pre-populate fields from existing item
                var items = await _itemService.GetItemsForListAsync(ListId);
                var existing = items.FirstOrDefault(i => i.Id == ItemId);
                if (existing is not null)
                {
                    Name = existing.Name;
                    Quantity = existing.Quantity ?? string.Empty;
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == existing.CategoryId)
                                       ?? Categories.FirstOrDefault();
                }
            }
            else
            {
                Title    = "Add Item";
                Subtitle = "Add item to your list";
                SelectedCategory = Categories.FirstOrDefault(c => c.Name == "Other")
                                   ?? Categories.FirstOrDefault();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    async Task SaveAsync()
    {
        if (!CanSave()) return;
        IsBusy = true;
        try
        {
            int categoryId = SelectedCategory?.Id ?? 8;

            if (IsEditMode)
            {
                var items = await _itemService.GetItemsForListAsync(ListId);
                var existing = items.FirstOrDefault(i => i.Id == ItemId);
                if (existing is not null)
                {
                    existing.Name = Name.Trim();
                    existing.Quantity = Quantity.Trim();
                    existing.CategoryId = categoryId;
                    await _itemService.UpdateItemAsync(existing);
                }
            }
            else
            {
                await _itemService.AddItemAsync(ListId, Name.Trim(), categoryId, Quantity.Trim());
            }

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    static async Task CancelAsync() =>
        await Shell.Current.GoToAsync("..");

    bool CanSave() => !string.IsNullOrWhiteSpace(Name);
}
