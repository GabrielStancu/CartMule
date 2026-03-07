# Wash — History

## Core Context

**Project:** CartMule — local-only shopping list manager in .NET 9 MAUI  
**Requested by:** Gabriel  
**My role:** Tech Lead — NuGet packages, folder structure, base classes, DI wiring, AppShell routes, chunk coordination

**Project file:** `D:\Projects\CartMule\src\CartMule\CartMule\CartMule.csproj`  
**Solution root:** `D:\Projects\CartMule\src\CartMule\`  
**App namespace:** `CartMule`

**Starting state (2026-03-07):** Default MAUI template. No NuGet packages added yet. Default purple theme. Single MainPage. OpenSans fonts registered. `GlobalXmlns.cs` maps `CartMule` and `CartMule.Pages` namespaces.

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

### 2026-03-07 — Chunk 1: Foundation
- Added NuGet packages: sqlite-net-pcl 1.9.172, SQLitePCLRaw.bundle_green 2.1.10, CommunityToolkit.Mvvm 8.3.2 to CartMule.csproj in a separate ItemGroup.
- Created ViewModels/BaseViewModel.cs using ObservableObject partial pattern with IsBusy/IsNotBusy/Title properties.
- Updated GlobalXmlns.cs to register CartMule.ViewModels, CartMule.Views, and CartMule.Converters namespaces.
- Updated MauiProgram.cs to file-scoped namespace, added DatabasePath static readonly pointing to AppDataDirectory/cartmule.db3.
- Scaffolded empty folders: Models/, Data/, Services/, ViewModels/, Views/, Converters/ via .gitkeep files.
