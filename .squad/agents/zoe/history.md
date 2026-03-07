# Zoe — History

## Core Context

**Project:** CartMule — local-only shopping list manager in .NET 9 MAUI  
**Requested by:** Gabriel  
**My role:** Backend Dev — SQLite models, repositories, services, ViewModels, DI wiring

**Architecture:**
- Clean MVVM: Models → Repositories → Services → ViewModels → XAML (Kaylee)
- SQLite-net-PCL for local persistence (async API)
- CommunityToolkit.Mvvm preferred for ObservableProperty + RelayCommand (check .csproj)
- All DI registration in `MauiProgram.cs`

**Data models:**
- `ShoppingList`: Id (int PK), Name (string), CreatedAt (DateTime), UpdatedAt (DateTime)
- `ShoppingItem`: Id (int PK), ListId (int FK), Name (string), CategoryId (int FK), Quantity (string), IsBought (bool), SortOrder (int)
- `Category`: Id (int PK), Name (string), SortOrder (int) — seeded on first run

**Sorting rule:** `IsBought = false` items first → grouped by `Category.SortOrder` → then by `ShoppingItem.SortOrder`. Bought items sink to bottom in real-time when toggled.

**Uncheck All:** Sets all `IsBought = false` on all items in a list, then refreshes sorted order.

**Key feature contracts for Kaylee:**
- `ListDetailViewModel.Items` → grouped collection, auto-updates on toggle
- `ListsDashboardViewModel.Lists` → includes `ItemCount` and `UpdatedAt` display string

**Team context:**
- Mal → Lead (reviews my service contracts and ViewModel shape)
- Kaylee → consumes my ViewModels and binds to them in XAML

## Learnings

### Chunk 1 — Domain Models (2026-03-07)

**Files created** (all in namespace `CartMule.Models`, folder `src/CartMule/CartMule/Models/`):

- `Category.cs` — `[PrimaryKey, AutoIncrement] Id`, `[NotNull] Name`, `SortOrder (int)`.
  Includes `static readonly IReadOnlyList<Category> Defaults` with 8 seeded entries (Produce→Other).
  `DatabaseContext` must insert these on first run (check if table is empty before seeding).
  "Other" has `Id = 8, SortOrder = 99` so it always sorts last.

- `ShoppingList.cs` — `[PrimaryKey, AutoIncrement] Id`, `[NotNull] Name`, `CreatedAt (DateTime)`,
  `UpdatedAt (DateTime)`. Both timestamps default to `DateTime.UtcNow` at construction.

- `ShoppingItem.cs` — `[PrimaryKey, AutoIncrement] Id`, `[NotNull, Indexed] ListId (int FK)`,
  `[NotNull] Name`, `CategoryId (int)` defaults to `8` (Other), `Quantity (string)` free-text,
  `IsBought (bool)`, `SortOrder (int)`.

**SQLite-net-PCL attribute conventions used:**
- `[PrimaryKey, AutoIncrement]` on all Id columns.
- `[NotNull]` on Name and required FK columns.
- `[Indexed]` on `ShoppingItem.ListId` for fast per-list queries.

**Sorting rule reminder:** IsBought ASC → Category.SortOrder ASC → ShoppingItem.SortOrder ASC.

## Learnings

### Chunk 2 — Data Layer (2026-03-07)
- DatabaseContext: lazy async init via GetConnectionAsync(); seeds Category.Defaults on empty table
- IListRepository/ListRepository: full CRUD + ordered by UpdatedAt desc
- IItemRepository/ItemRepository: includes DeleteByListIdAsync (raw SQL) and GetCountByListIdAsync
- ICategoryRepository/CategoryRepository: read-only, sorted by SortOrder
- IShoppingListService/ShoppingListService: Create/Rename/Delete (cascades items) /Touch/GetItemCount
- IShoppingItemService/ShoppingItemService: GetItemsForListAsync sorts IsBought ASC → catSortOrder ASC → SortOrder ASC
- ToggleBoughtAsync and UncheckAllAsync both call TouchListAsync to keep UpdatedAt current
- ICategoryService/CategoryService: thin wrapper over ICategoryRepository for VM consumption

### Chunk 3 — ListsDashboardViewModel (2026-03-07)
- ListSummaryItem is a display DTO (public sealed class) defined alongside the ViewModel in ViewModels/
- ObservableCollection<ListSummaryItem> is not ObservableProperty — mutated directly (Clear/Add/Remove)
- IsEmpty is ObservableProperty; IsNotEmpty is computed via NotifyPropertyChangedFor
- LoadListsAsync uses IsBusy guard; sets IsEmpty=false at start to hide empty state during load
- CreateListAsync is a plain public async method called from page code-behind after DisplayPromptAsync
- DeleteListAsync removes from the in-memory collection immediately (no full reload needed)
- OpenListCommand is a placeholder stub for Chunk 4 navigation

### Chunk 4 — ListDetailViewModel (2026-03-07)
- ItemGroup : ObservableCollection<ShoppingItem> with Name + IsBoughtGroup; defined in same file as ViewModel
- [QueryProperty(nameof(ListId), "id")] — Shell sets ListId after construction; LoadItemsCommand triggered from OnAppearing
- RebuildGroups: unbought items grouped by CategoryId (service sort order preserved), all bought items → single "In Cart ✓" group at bottom
- IsEmpty/IsNotEmpty: controlled in RebuildGroups and DeleteItemAsync
- DeleteItemAsync removes locally (no full reload) and cleans up empty groups
- ToggleBoughtAsync and UncheckAllAsync do a full LoadItemsAsync reload to re-sort correctly

### Chunk 5 — AddItemViewModel (2026-03-07)
- Two QueryProperty: "listId" → ListId (int), "itemId" → ItemId (int; 0 means add mode)
- IsEditMode = ItemId > 0 (computed, no ObservableProperty needed — value is stable after navigation)
- InitialiseCommand: loads categories, pre-populates fields in edit mode, sets default category to "Other" in add mode
- SaveCommand has CanExecute guarded by CanSave() = !IsNullOrWhiteSpace(Name); NotifyCanExecuteChangedFor fires on Name changes
- In edit mode, SaveAsync fetches the live item from service (to avoid stale state), patches fields, then UpdateItemAsync
- CancelCommand is a static relay command (no instance state needed)
- CategoryId fallback: 8 (Other) if SelectedCategory is null
