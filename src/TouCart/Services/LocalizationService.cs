using System.ComponentModel;

namespace TouCart.Services;

public sealed class LocalizationService : ILocalizationService
{
    private string _code;

    public LocalizationService()
    {
        _code = Preferences.Default.Get("app_language", "en");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    // Fires "" to invalidate ALL properties at once — every {Binding Loc.Xxx} refreshes.
    private void NotifyAll() =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));

    public string CurrentLanguage  => _code;
    public string CurrentFlagEmoji => _code == "ro" ? "🇷🇴" : "🇬🇧";

    public void SetLanguage(string code)
    {
        if (_code == code) return;
        _code = code;
        Preferences.Default.Set("app_language", code);
        NotifyAll();
    }

    private bool Ro => _code == "ro";

    // ── Dashboard ─────────────────────────────────────────────────────────────
    public string Hello                  => Ro ? "Salut!"                      : "Hello!";
    public string WhatDoYouNeedToday     => Ro ? "De ce ai nevoie astăzi?"        : "What do you need today?";
    public string ShoppingLists          => Ro ? "Liste de cumpărături"         : "Shopping Lists";
    public string SearchListsPlaceholder => Ro ? "Caută liste..."               : "Search lists...";
    public string NoListsYet             => Ro ? "Nicio listă"                  : "No lists yet";
    public string TapToGetStarted        => Ro ? "Apasă + pentru a începe"      : "Tap + above to get started";
    public string Delete                 => Ro ? "Șterge"                       : "Delete";
    public string Cancel                 => Ro ? "Anulează"                     : "Cancel";
    public string DeleteListTitle        => Ro ? "Ștergi această listă?"         : "Delete this list?";
    public string DeleteListMessage      => Ro ? "Toate articolele din această listă vor fi șterse permanent."
                                               : "All items in this list will also be permanently deleted.";
    public string New                    => Ro ? "Nou"                          : "New";

    public string FormatItemCount(int count) =>
        Ro ? $"{count} articole" : $"{count} items";

    // ── List Detail ───────────────────────────────────────────────────────────
    public string FilterItemsPlaceholder => Ro ? "Filtrează articole..."         : "Filter items...";
    public string NoItemsFound           => Ro ? "Niciun articol găsit"          : "No items found";
    public string NoItemsYet             => Ro ? "Niciun articol"                : "No items yet";
    public string TapToAddItem           => Ro ? "Apasă + pentru a adăuga"       : "Tap + to add an item";
    public string InCart                 => Ro ? "În coș ✓"                      : "In Cart ✓";
    public string Edit                   => Ro ? "Editează"                      : "Edit";
    public string DeleteItemTitle        => Ro ? "Ștergi articolul?"             : "Delete item?";
    public string DeleteItemMessage      => Ro ? "Articolul va fi eliminat permanent din listă."
                                               : "This item will be permanently removed from the list.";
    public string Other                  => Ro ? "Altele"                        : "Other";

    public string FormatUpdated(DateTime dt) =>
        Ro ? $"Actualizat {dt.ToLocalTime():d MMM, H:mm}"
           : $"Updated {dt.ToLocalTime():MMM d, H:mm}";

    // ── Add / Edit Item ───────────────────────────────────────────────────────
    public string AddItemTitle         => Ro ? "Adaugă articol"              : "Add Item";
    public string EditItemTitle        => Ro ? "Editează articol"            : "Edit Item";
    public string AddItemSubtitle      => Ro ? "Adaugă articol în lista ta"  : "Add item to your list";
    public string EditItemSubtitle     => Ro ? "Actualizează detaliile"      : "Update item details";
    public string ItemNameLabel        => Ro ? "Denumire articol"            : "Item Name";
    public string ItemNamePlaceholder  => Ro ? "ex. Lapte integral"          : "e.g. Whole milk";
    public string QuantityLabel        => Ro ? "Cantitate (opțional)"        : "Quantity (optional)";
    public string QuantityPlaceholder  => Ro ? "ex. 2 litri"                 : "e.g. 2 litres";
    public string CategoryLabel        => Ro ? "Categorie"                   : "Category";
    public string Save                 => Ro ? "Salvează"                    : "Save";

    // ── Add / Edit List ───────────────────────────────────────────────────────
    public string NewListTitle           => Ro ? "Listă nouă"                        : "New List";
    public string EditListTitle          => Ro ? "Editează lista"                    : "Edit List";
    public string NewListSubtitle        => Ro ? "Creează o listă nouă"              : "Create a new shopping list";
    public string EditListSubtitle       => Ro ? "Actualizează detaliile"            : "Update list details";
    public string ListNameLabel          => Ro ? "Denumire listă"                    : "List Name";
    public string ListNamePlaceholder    => Ro ? "ex. Cumpărături săptămânale"       : "e.g. Weekly Groceries";
    public string ShopsLabel             => Ro ? "Magazine (opțional)"              : "Shops (optional)";
    public string AddShopPlaceholder     => Ro ? "Adaugă un magazin…"               : "Add a shop…";
    public string CategoriesLabel        => Ro ? "Categorii"                        : "Categories";
    public string AddCategoryPlaceholder => Ro ? "Adaugă o categorie…"              : "Add a category…";
    public string DeleteListBtn          => Ro ? "Șterge lista"                     : "Delete List";
    public string EditModalTitle         => Ro ? "Editează"                         : "Edit";

    // ── Language picker ───────────────────────────────────────────────────────
    public string ChooseLanguage => Ro ? "Alege Limba" : "Choose Language";

    // ── Category name translation ─────────────────────────────────────────────
    public string TranslateCategoryName(string englishName) => Ro ? englishName switch
    {
        "Produce"   => "Legume și Fructe",
        "Dairy"     => "Lactate",
        "Bakery"    => "Panificație",
        "Meat"      => "Carne",
        "Beverages" => "Băuturi",
        "Frozen"    => "Congelate",
        "Household" => "Menaj",
        "Other"     => "Altele",
        _           => englishName,
    } : englishName;
}
