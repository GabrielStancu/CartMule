# Mal — History

## Core Context

**Project:** TouCart — a local-only shopping list manager in .NET 9 MAUI  
**Requested by:** Gabriel  
**My role:** Lead & Architect — project structure, MVVM wiring, code review, go/no-go calls

**Stack:** C# · .NET 9 MAUI · SQLite-net-PCL · Clean MVVM · DI via MauiProgram.cs  
**Target platforms:** Android, iOS, macOS Catalyst, Windows  
**Visual style:** Flat/minimalist — Yellow #FFD200, White #FFFFFF, Dark Grey #1A1A1A

**Key features:**
- Multi-list dashboard (create, rename, delete lists; show item count + last-updated timestamp)
- Item management: add/edit/remove items, support Categories (Produce, Dairy, Bakery, etc.)
- Shopping Mode: checked items get strikethrough + animate to bottom of list in real-time
- Auto-reset: "Uncheck All" returns all items to Not Bought, restores category sorting

**Team:**
- Kaylee → UI/XAML (pages, animations, design system)
- Zoe → Backend (SQLite, repos, services, ViewModels)
- Scribe → Memory and logs
- Ralph → Work queue monitor

## Learnings

*(Append new learnings here as work progresses)*
