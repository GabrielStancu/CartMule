using System.ComponentModel;

namespace TouCart.Services;

public interface ILocalizationService : INotifyPropertyChanged
{
    string CurrentLanguage    { get; }
    string CurrentFlagEmoji   { get; }
    void   SetLanguage(string code);

    // ── Dashboard ─────────────────────────────────────────────────────────────
    string Hello                   { get; }
    string WhatDoYouNeedToday      { get; }
    string ShoppingLists           { get; }
    string SearchListsPlaceholder  { get; }
    string NoListsYet              { get; }
    string TapToGetStarted         { get; }
    string Delete                  { get; }
    string Cancel                  { get; }
    string DeleteListTitle         { get; }
    string DeleteListMessage       { get; }
    string New                     { get; }
    string FormatItemCount(int count);

    // ── List Detail ───────────────────────────────────────────────────────────
    string FilterItemsPlaceholder  { get; }
    string NoItemsFound            { get; }
    string NoItemsYet              { get; }
    string TapToAddItem            { get; }
    string InCart                  { get; }
    string Edit                    { get; }
    string DeleteItemTitle         { get; }
    string DeleteItemMessage       { get; }
    string Other                   { get; }
    string FormatUpdated(DateTime dt);

    // ── Add / Edit Item ───────────────────────────────────────────────────────
    string AddItemTitle            { get; }
    string EditItemTitle           { get; }
    string AddItemSubtitle         { get; }
    string EditItemSubtitle        { get; }
    string ItemNameLabel           { get; }
    string ItemNamePlaceholder     { get; }
    string QuantityLabel           { get; }
    string QuantityPlaceholder     { get; }
    string CategoryLabel           { get; }
    string Save                    { get; }

    // ── Add / Edit List ───────────────────────────────────────────────────────
    string NewListTitle            { get; }
    string EditListTitle           { get; }
    string NewListSubtitle         { get; }
    string EditListSubtitle        { get; }
    string ListNameLabel           { get; }
    string ListNamePlaceholder     { get; }
    string ShopsLabel              { get; }
    string AddShopPlaceholder      { get; }
    string CategoriesLabel         { get; }
    string AddCategoryPlaceholder  { get; }
    string DeleteListBtn           { get; }
    string EditModalTitle          { get; }

    // ── Language picker ───────────────────────────────────────────────────────
    string ChooseLanguage          { get; }

    // ── Category name translation ─────────────────────────────────────────────
    /// <summary>Translates a seeded English category name to the current language.
    /// Custom (user-added) names are returned unchanged.</summary>
    string TranslateCategoryName(string englishName);
}
