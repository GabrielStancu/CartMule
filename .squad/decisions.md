# Squad Decisions

## Active Decisions

### 2026-03-07: Architecture — Clean MVVM with Repository Pattern
**By:** Gabriel (via Coordinator)  
**What:** CartMule uses Clean MVVM. Layer order: Models → Repositories (interfaces + SQLite-net-PCL impls) → Services → ViewModels → XAML Pages. No direct DB access from ViewModels.  
**Why:** Separation of concerns; supports future testability; requested by user.

---

### 2026-03-07: Storage — SQLite-net-PCL, Local-Only
**By:** Gabriel (via Coordinator)  
**What:** All data is stored locally using SQLite-net-PCL. No cloud sync, no REST API, no network calls.  
**Why:** Explicitly local-only per requirements.

---

### 2026-03-07: UI Design System — Yellow/White/Dark Grey Flat Aesthetic
**By:** Gabriel (via Coordinator)  
**What:** Color palette locked: `PrimaryYellow #FFD200`, `SurfaceWhite #FFFFFF`, `TextCharcoal #1A1A1A`. Font: Montserrat or Open Sans. Corner radius ~20–24px on cards, ~16px on inputs. FAB: yellow, bottom-right. No shadow/elevation effects — flat design.  
**Why:** Direct user requirement based on reference UI image.

---

### 2026-03-07: Item Sort Rule — Bought Items Sink to Bottom
**By:** Gabriel (via Coordinator)  
**What:** When `ShoppingItem.IsBought` is toggled to `true`, the item animates to the bottom of the list in real-time. Sort order: `IsBought ASC → Category.SortOrder ASC → ShoppingItem.SortOrder ASC`. Uncheck All resets all items to `IsBought = false` and restores category ordering.  
**Why:** Core "Shopping Mode" feature per requirements.

---

### 2026-03-07: DI — MauiProgram.cs is the Composition Root
**By:** Gabriel (via Coordinator)  
**What:** All services, repositories, viewmodels, and pages are registered in `MauiProgram.cs`. No service locator pattern. Constructor injection throughout.  
**Why:** Clean DI, testable, idiomatic MAUI pattern.

---

### 2026-03-07: Chunk 1 — Foundation complete
**By:** Wash (Tech Lead)
**What:** Added sqlite-net-pcl 1.9.172, SQLitePCLRaw.bundle_green 2.1.10, CommunityToolkit.Mvvm 8.3.2. Created BaseViewModel using ObservableObject partial pattern. Established folder structure. DatabasePath constant in MauiProgram.cs points to AppDataDirectory/cartmule.db3.
**Why:** Foundation for all subsequent chunks.

---

### 2026-03-07: Chunk 2 — Data Layer complete
**By:** Zoe (Backend Dev)
**What:** DatabaseContext with lazy async init and category seeding. Full repository layer (IListRepository, IItemRepository, ICategoryRepository). Full service layer (IShoppingListService, IShoppingItemService, ICategoryService). Sorting rule implemented in ShoppingItemService.GetItemsForListAsync: IsBought ASC → Category.SortOrder ASC → ShoppingItem.SortOrder ASC.
**Why:** Foundation for ViewModels in Chunk 3+.

---

### 2026-03-07: Chunk 3 — ListsDashboardViewModel
**By:** Zoe (Backend Dev)
**What:** ListSummaryItem display DTO + ListsDashboardViewModel with LoadListsCommand, CreateListAsync, DeleteListCommand, OpenListCommand. OpenListCommand is a placeholder stub for Chunk 4.
**Why:** Dashboard requires per-item count which requires an N+1 call per list — acceptable since list count is expected to be small.

---

### 2026-03-07: Chunk 4 — ListDetailViewModel
**By:** Zoe (Backend Dev)
**What:** ItemGroup (ObservableCollection<ShoppingItem> subclass) + ListDetailViewModel with LoadItemsCommand, ToggleBoughtCommand, DeleteItemCommand, UncheckAllCommand. Items grouped: unbought by category, bought → single "In Cart ✓" group at bottom. QueryProperty("id") feeds ListId from Shell navigation.
**Why:** Grouping pattern chosen to clearly show shopping progress; "In Cart" bucket gives user visual confirmation of what's already in the cart.

---

### 2026-03-07: Chunk 5 — AddItemViewModel
**By:** Zoe (Backend Dev)
**What:** AddItemViewModel with dual QueryProperty (listId + itemId). Add mode: creates new item. Edit mode (itemId > 0): pre-populates fields and updates existing. SaveCommand guarded by CanSave (name required). InitialiseCommand loads categories and pre-populates if edit mode.
**Why:** Single VM handles both add and edit to avoid code duplication. InitialiseCommand called from OnNavigatedTo (not OnAppearing) to avoid re-population on back-nav.

---

## Governance

- All meaningful architectural changes require Mal's approval before implementation
- Kaylee may not change the data model; coordinate with Zoe via drop-box decisions
- Append decisions to this file chronologically; never delete old entries
