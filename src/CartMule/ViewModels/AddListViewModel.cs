using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CartMule.Services;

namespace CartMule.ViewModels;

public partial class CategoryEditItem : ObservableObject
{
    public int Id { get; init; }  // 0 = not yet persisted

    [ObservableProperty]
    string _name = string.Empty;
}

[QueryProperty(nameof(ListId), "listId")]
public partial class AddListViewModel : BaseViewModel
{
    private readonly IShoppingListService _listService;
    private readonly ICategoryService     _categoryService;

    public int  ListId       { get; set; }
    public bool IsCreateMode => ListId == 0;
    public bool IsEditMode   => ListId != 0;

    [ObservableProperty]
    string _name = string.Empty;

    [ObservableProperty]
    string _shops = string.Empty;

    [ObservableProperty]
    string _newCategoryName = string.Empty;

    [ObservableProperty]
    bool _showDeleteListConfirm;

    public ObservableCollection<CategoryEditItem> Categories { get; } = new();

    public AddListViewModel(IShoppingListService listService, ICategoryService categoryService)
    {
        _listService     = listService;
        _categoryService = categoryService;
    }

    [RelayCommand]
    async Task InitialiseAsync()
    {
        Title = IsCreateMode ? "New List" : "Edit List";

        var categories = await _categoryService.GetAllCategoriesAsync();
        Categories.Clear();
        foreach (var c in categories)
            Categories.Add(new CategoryEditItem { Id = c.Id, Name = c.Name });

        if (IsEditMode)
        {
            var list = await _listService.GetListByIdAsync(ListId);
            Name  = list?.Name  ?? string.Empty;
            Shops = list?.Shops ?? string.Empty;
        }
    }

    [RelayCommand]
    async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;

        if (IsCreateMode)
            await _listService.CreateListAsync(Name.Trim(), Shops.Trim());
        else
            await _listService.UpdateListAsync(ListId, Name.Trim(), Shops.Trim());

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    static async Task CancelAsync() => await Shell.Current.GoToAsync("..");

    // ── Category management ──────────────────────────────────────────────────

    [RelayCommand]
    async Task AddCategoryAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName)) return;
        var created = await _categoryService.CreateCategoryAsync(NewCategoryName.Trim());
        Categories.Add(new CategoryEditItem { Id = created.Id, Name = created.Name });
        NewCategoryName = string.Empty;
    }

    [RelayCommand]
    async Task DeleteCategoryAsync(CategoryEditItem item)
    {
        if (item.Id != 0)
            await _categoryService.DeleteCategoryAsync(item.Id);
        Categories.Remove(item);
    }

    /// <summary>Called from code-behind after DisplayPromptAsync.</summary>
    public async Task RenameCategoryAsync(CategoryEditItem item, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName) || newName == item.Name) return;
        if (item.Id != 0)
            await _categoryService.UpdateCategoryAsync(item.Id, newName.Trim());
        item.Name = newName.Trim();
    }

    // ── Delete list (edit mode only) ─────────────────────────────────────────

    [RelayCommand]
    void RequestDeleteList() => ShowDeleteListConfirm = true;

    [RelayCommand]
    async Task ConfirmDeleteListAsync()
    {
        ShowDeleteListConfirm = false;
        await _listService.DeleteListAsync(ListId);
        // Pop AddListPage and ListDetailPage back to dashboard
        await Shell.Current.GoToAsync("../..");
    }

    [RelayCommand]
    void CancelDeleteList() => ShowDeleteListConfirm = false;
}
