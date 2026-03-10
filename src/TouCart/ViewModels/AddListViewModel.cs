using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TouCart.Services;

namespace TouCart.ViewModels;

public partial class CategoryEditItem : ObservableObject
{
    public int Id { get; set; }  // 0 = not yet persisted; mutable after DB insert

    [ObservableProperty]
    string _name = string.Empty;

    internal Action<CategoryEditItem>? NameChanged;
    partial void OnNameChanged(string value) => NameChanged?.Invoke(this);
}

public partial class ShopEditItem : ObservableObject
{
    [ObservableProperty]
    string _name = string.Empty;

    internal Action<ShopEditItem>? NameChanged;
    partial void OnNameChanged(string value) => NameChanged?.Invoke(this);
}

[QueryProperty(nameof(ListId), "listId")]
public partial class AddListViewModel : BaseViewModel
{
    private readonly IShoppingListService _listService;
    private readonly ICategoryService     _categoryService;
    private readonly ILocalizationService _loc;

    public ILocalizationService Loc => _loc;

    public int  ListId       { get; set; }
    public bool IsCreateMode => ListId == 0;
    public bool IsEditMode   => ListId != 0;

    [ObservableProperty]
    string _name = string.Empty;

    [ObservableProperty]
    string _subtitle = string.Empty;

    [ObservableProperty]
    bool _showDeleteListConfirm;

    [ObservableProperty]
    bool _showEditModal;

    [ObservableProperty]
    string _modalEditName = string.Empty;

    private CategoryEditItem? _modalCategory;
    private ShopEditItem?     _modalShop;

    public ObservableCollection<CategoryEditItem> Categories { get; } = new();
    public ObservableCollection<ShopEditItem>     Shops      { get; } = new();

    public string ShopsDisplay =>
        string.Join(", ", Shops
            .Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .Select(s => s.Name.Trim()));

    public AddListViewModel(IShoppingListService listService, ICategoryService categoryService, ILocalizationService loc)
    {
        _listService     = listService;
        _categoryService = categoryService;
        _loc             = loc;
    }

    [RelayCommand]
    async Task InitialiseAsync()
    {
        Title    = IsCreateMode ? _loc.NewListTitle   : _loc.EditListTitle;
        Subtitle = IsCreateMode ? _loc.NewListSubtitle : _loc.EditListSubtitle;

        var categories = await _categoryService.GetAllCategoriesAsync();
        Categories.Clear();
        foreach (var c in categories)
            AddCategoryItem(new CategoryEditItem { Id = c.Id, Name = _loc.TranslateCategoryName(c.Name) });
        EnsureBlankTrailingCategory();

        Shops.Clear();
        if (IsEditMode)
        {
            var list = await _listService.GetListByIdAsync(ListId);
            Name = list?.Name ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(list?.Shops))
            {
                foreach (var shop in list.Shops.Split(',')
                             .Select(s => s.Trim())
                             .Where(s => s.Length > 0))
                    AddShopItem(new ShopEditItem { Name = shop });
            }
        }
        EnsureBlankTrailingShop();
    }

    [RelayCommand]
    async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;

        var shopsString = ShopsDisplay;

        if (IsCreateMode)
            await _listService.CreateListAsync(Name.Trim(), shopsString);
        else
            await _listService.UpdateListAsync(ListId, Name.Trim(), shopsString);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    static async Task CancelAsync() => await Shell.Current.GoToAsync("..");

    // ── Category management ──────────────────────────────────────────────────

    private void AddCategoryItem(CategoryEditItem item)
    {
        item.NameChanged = OnCategoryNameChanged;
        Categories.Add(item);
    }

    private void OnCategoryNameChanged(CategoryEditItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Name) && Categories.Last() == item)
            EnsureBlankTrailingCategory();
    }

    private void EnsureBlankTrailingCategory()
    {
        if (Categories.Count == 0 || !string.IsNullOrWhiteSpace(Categories.Last().Name))
        {
            var blank = new CategoryEditItem();
            blank.NameChanged = OnCategoryNameChanged;
            Categories.Add(blank);
        }
    }

    private void TrimExtraBlanksCategory()
    {
        var blanks = Categories.Where(c => string.IsNullOrWhiteSpace(c.Name)).ToList();
        for (int i = 0; i < blanks.Count - 1; i++)
            Categories.Remove(blanks[i]);
    }

    private void TrimExtraBlanksShop()
    {
        var blanks = Shops.Where(s => string.IsNullOrWhiteSpace(s.Name)).ToList();
        for (int i = 0; i < blanks.Count - 1; i++)
            Shops.Remove(blanks[i]);
    }

    public async Task PersistCategoryOnFocusLostAsync(CategoryEditItem item)
    {
        var name = item.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            TrimExtraBlanksCategory();
            return;
        }

        if (item.Id == 0)
        {
            var created = await _categoryService.CreateCategoryAsync(name);
            item.Id = created.Id;
        }
        else
        {
            await _categoryService.UpdateCategoryAsync(item.Id, name);
        }
    }

    // ── Shop management ──────────────────────────────────────────────────────

    private void AddShopItem(ShopEditItem item)
    {
        item.NameChanged = OnShopNameChanged;
        Shops.Add(item);
    }

    private void OnShopNameChanged(ShopEditItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Name) && Shops.Last() == item)
            EnsureBlankTrailingShop();
    }

    private void EnsureBlankTrailingShop()
    {
        if (Shops.Count == 0 || !string.IsNullOrWhiteSpace(Shops.Last().Name))
        {
            var blank = new ShopEditItem();
            blank.NameChanged = OnShopNameChanged;
            Shops.Add(blank);
        }
    }

    public async Task PersistShopOnFocusLostAsync(ShopEditItem item)
    {
        var name = item.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
            TrimExtraBlanksShop();
        if (!IsEditMode) return;
        var shopsString = ShopsDisplay;
        await _listService.UpdateListAsync(ListId, Name.Trim(), shopsString);
    }

    // ── Edit modal (shared for both categories and shops) ────────────────────

    [RelayCommand]
    void OpenCategoryMenu(CategoryEditItem item)
    {
        _modalCategory = item;
        _modalShop     = null;
        ModalEditName  = item.Name;
        ShowEditModal  = true;
    }

    [RelayCommand]
    void OpenShopMenu(ShopEditItem item)
    {
        _modalShop     = item;
        _modalCategory = null;
        ModalEditName  = item.Name;
        ShowEditModal  = true;
    }

    [RelayCommand]
    async Task SaveEditModalAsync()
    {
        var name = ModalEditName.Trim();
        ShowEditModal = false;

        if (_modalCategory is not null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _modalCategory.Name = name;
                await PersistCategoryOnFocusLostAsync(_modalCategory);
            }
            _modalCategory = null;
        }
        else if (_modalShop is not null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _modalShop.Name = name;
                await PersistShopOnFocusLostAsync(_modalShop);
            }
            _modalShop = null;
        }
    }

    [RelayCommand]
    async Task DeleteEditModalItemAsync()
    {
        ShowEditModal = false;

        if (_modalCategory is not null)
        {
            var cat = _modalCategory;
            _modalCategory = null;
            if (cat.Id != 0)
                await _categoryService.DeleteCategoryAsync(cat.Id);
            Categories.Remove(cat);
            EnsureBlankTrailingCategory();
        }
        else if (_modalShop is not null)
        {
            var shop = _modalShop;
            _modalShop = null;
            Shops.Remove(shop);
            EnsureBlankTrailingShop();
            if (IsEditMode)
                await _listService.UpdateListAsync(ListId, Name.Trim(), ShopsDisplay);
        }
    }

    [RelayCommand]
    void CancelEditModal()
    {
        ShowEditModal  = false;
        _modalCategory = null;
        _modalShop     = null;
    }

    // ── Delete list (edit mode only) ─────────────────────────────────────────

    [RelayCommand]
    void RequestDeleteList() => ShowDeleteListConfirm = true;

    [RelayCommand]
    async Task ConfirmDeleteListAsync()
    {
        ShowDeleteListConfirm = false;
        if (IsEditMode)
            await _listService.DeleteListAsync(ListId);
        await Shell.Current.GoToAsync("../..");
    }

    [RelayCommand]
    void CancelDeleteList() => ShowDeleteListConfirm = false;
}
