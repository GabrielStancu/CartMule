# Kaylee — History

## Core Context

**Project:** CartMule — local-only shopping list manager in .NET 9 MAUI  
**Requested by:** Gabriel  
**My role:** UI Dev — XAML pages, design system, animations, flat/minimalist aesthetic

**Visual Language:**
- Primary accent: Yellow `#FFD200`
- Background/surface: White `#FFFFFF`
- Text/dark elements: Dark Grey/Black `#1A1A1A`
- Rounded corners: ~20–24px on cards, ~16px on inputs
- Font: Montserrat or Open Sans (clean sans-serif)
- FAB: Yellow circle, bottom-right, dark icon, large shadow-free

**Pages to build:**
- `ListsDashboardPage` — cards for each shopping list (name, item count, last updated)
- `ListDetailPage` — items grouped by category, FAB to add, "Uncheck All" button
- `AddEditItemPage` — name, category picker, quantity input

**Animation requirements:**
- Checked item: strikethrough appears → item smoothly slides and fades to bottom
- List entry: subtle slide-in on page load
- FAB: subtle scale on tap

**Reference image:** Polish grocery app with yellow header, white list cards, clean typography, minimal icons — very similar to Biedronka-style shopping UX

**Team context:**
- Mal → Lead (reviews my XAML, sets contracts for VM bindings)
- Zoe → provides ViewModels and services I bind to

## Learnings

- Fonts: Design system uses `OpenSansSemibold` (headings, buttons, labels) and `OpenSansRegular` (body, inputs). Both are already registered in `MauiProgram.cs` — do not re-register.
- Named style keys (for use in XAML via `Style="{StaticResource ...}"`):
  - `PageTitleLabel` — 28px semibold, TextCharcoal, tight letter-spacing
  - `SectionSubtitleLabel` — 13px regular, SubtleGray
  - `ItemNameLabel` — 16px regular, TextCharcoal (active item)
  - `BoughtItemNameLabel` — 16px regular, SubtleGray, Strikethrough (purchased item)
  - `CategoryHeaderLabel` — 12px semibold, SubtleGray, wide letter-spacing
  - `ListCardBorder` — white rounded card (radius 20), no stroke, padding 20,18, margin 16,6
  - `ListCardTitleLabel` — 17px semibold, TextCharcoal
  - `ListCardMetaLabel` — 13px regular, SubtleGray
  - `FABButton` — 56×56 yellow circle (radius 28), 28px icon, shadow 0,4 / 0.15 / r8
  - `RoundedInputField` — Entry: white bg, 16px, h52
  - `RoundedPicker` — Picker: white bg, 16px, h52
  - `InputBorder` — Border wrapping inputs: LightGray stroke 1.5, radius 14, padding 16,0
  - `DangerButton` — transparent bg, DangerRed text, no border
  - `GhostButton` — transparent bg, TextCharcoal, LightGray border 1.5, radius 14
  - `SeparatorLine` — BoxView: LightGray, 1px height, Fill
  - `CountBadgeBorder` — yellow pill (radius 10), padding 10,4
  - `CountBadgeLabel` — 12px semibold, TextCharcoal
- Converters (namespace `CartMule.Converters`):
  - `BoolToTextDecorationConverter` — `true` → `Strikethrough`, `false` → `None`
  - `BoolToOpacityConverter` — `true` → `0.45` (dimmed/bought), `false` → `1.0` (active)
- Color keys: `PrimaryYellow` (#FFD200), `TextCharcoal` (#1A1A1A), `SubtleGray` (#757575), `LightGray` (#E8E8E8), `Surface` (#FFFFFF), `Background` (#F7F7F7), `DangerRed` (#FF3B30), `SuccessGreen` (#34C759)
- Corner radii: cards=20, inputs=14, FAB=28

### Chunk 3 — ListsDashboardPage (2026-03-07)
- Shell.NavBarIsVisible="False" — we own the header bar (yellow Border with Padding 20,52,20,20 for status bar)
- CollectionView Margin bottom 90 so FAB doesn't overlap last card
- SwipeView RightItems Mode="Execute" for delete swipe
- TapGestureRecognizer on card Border binds to OpenListCommand via RelativeSource AncestorType
- FAB uses Clicked event (code-behind) so DisplayPromptAsync can collect the list name
- x:DataType="vm:ListSummaryItem" on DataTemplate for compiled bindings; type lives in CartMule.ViewModels namespace

### Chunk 4 — ListDetailPage (2026-03-07)
- Page declares converters in ContentPage.Resources (BoolToTextDecoration, BoolToOpacity, StringNotEmpty)
- xmlns:models="clr-namespace:CartMule.Models" needed for x:DataType on item DataTemplate
- Grouped CollectionView: GroupHeaderTemplate uses x:DataType="vm:ItemGroup", ItemTemplate uses x:DataType="models:ShoppingItem"
- Circle checkbox: Border with Ellipse StrokeShape, DataTrigger fills it dark when IsBought=true
- ToggleBoughtCommand and DeleteItemCommand bound via RelativeSource AncestorType on ListDetailViewModel (same XC0025 warning as dashboard — harmless)
- OnAddItemClicked navigates to "additem?listId=..." — stub route for Chunk 5
- StringNotEmptyConverter: IValueConverter returning true when string is not null/whitespace — hides empty quantity labels
