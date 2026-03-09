# Wash — History

## Core Context

**Project:** TouCart — local-only shopping list manager in .NET 9 MAUI  
**Requested by:** Gabriel  
**My role:** Tech Lead — NuGet packages, folder structure, base classes, DI wiring, AppShell routes, chunk coordination

**Project file:** `D:\Projects\TouCart\src\TouCart\TouCart\TouCart.csproj`  
**Solution root:** `D:\Projects\TouCart\src\TouCart\`  
**App namespace:** `TouCart`

**Starting state (2026-03-07):** Default MAUI template. No NuGet packages added yet. Default purple theme. Single MainPage. OpenSans fonts registered. `GlobalXmlns.cs` maps `TouCart` and `TouCart.Pages` namespaces.

**NuGet packages needed (Chunk 1):**
- `sqlite-net-pcl` 1.9.172
- `SQLitePCLRaw.bundle_green` 2.1.10
- `CommunityToolkit.Mvvm` 8.3.2

**Folder structure to create:**
- `Models/`
- `Data/`
- `Services/`
- `ViewModels/`
- `Views/`
- `Converters/`

**Team:**
- Mal → Lead (my reviewer)
- Kaylee → UI Dev (I unblock her on structural issues)
- Zoe → Backend Dev (I wire up her services in MauiProgram.cs)
- Scribe → Logs
- Ralph → Monitor

## Learnings

*(Append new learnings here as work progresses)*

### Chunk 5 — Route + DI + Edit Nav (2026-03-07)
- AppShell.xaml.cs: Routing.RegisterRoute("additem", typeof(AddItemPage)) added alongside listdetail
- MauiProgram.cs: AddItemViewModel + AddItemPage both AddTransient
- ListDetailPage.xaml: double-tap gesture → EditItemCommand with item as CommandParameter
- ListDetailViewModel: EditItemAsync navigates GoToAsync("additem?listId=...&itemId=...") 
- Both listId and itemId passed as query params; AddItemViewModel's QueryProperty handles them
- TouCart is now feature-complete: all 5 chunks done

### Chunk 4 — Route + DI + Navigation (2026-03-07)
- AppShell.xaml.cs: Routing.RegisterRoute("listdetail", typeof(ListDetailPage)) — must be called in constructor before any navigation
- MauiProgram.cs: ListDetailViewModel + ListDetailPage both AddTransient (fresh instance on each navigation)
- ListsDashboardViewModel.OpenListAsync: GoToAsync("listdetail?id={id}") — Shell QueryProperty "id" feeds ListDetailViewModel.ListId
- Additem route ("additem?listId=...") is NOT registered yet — that is Chunk 5

### Chunk 3 — DI wiring + AppShell (2026-03-07)
- ListsDashboardViewModel and ListsDashboardPage registered as Transient (pages should be Transient for correct lifetime)
- AppShell simplified to single ShellContent pointing to ListsDashboardPage, Route="lists"
- Shell.NavBarIsVisible="False" on AppShell so the page controls its own header
- using TouCart.ViewModels + TouCart.Views added to MauiProgram.cs

### Chunk 2 — DI wiring (2026-03-07)
- MauiProgram.cs updated with full DI registrations
- Registration order: DatabaseContext (singleton, direct instance) → Repositories → Services
- All repos + services registered as Singleton (DB connection is also singleton — safe)
- No ViewModels or Pages registered yet — that is Chunk 3

### 2026-03-07 — Chunk 1: Foundation
- Added NuGet packages: sqlite-net-pcl 1.9.172, SQLitePCLRaw.bundle_green 2.1.10, CommunityToolkit.Mvvm 8.3.2 to TouCart.csproj in a separate ItemGroup.
- Created ViewModels/BaseViewModel.cs using ObservableObject partial pattern with IsBusy/IsNotBusy/Title properties.
- Updated GlobalXmlns.cs to register TouCart.ViewModels, TouCart.Views, and TouCart.Converters namespaces.
- Updated MauiProgram.cs to file-scoped namespace, added DatabasePath static readonly pointing to AppDataDirectory/TouCart.db3.
- Scaffolded empty folders: Models/, Data/, Services/, ViewModels/, Views/, Converters/ via .gitkeep files.
