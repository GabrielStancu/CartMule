using Microsoft.Extensions.Logging;
using CartMule.Data;
using CartMule.Services;
using CartMule.ViewModels;
using CartMule.Views;

namespace CartMule;

public static class MauiProgram
{
    public static readonly string DatabasePath =
        Path.Combine(FileSystem.AppDataDirectory, "cartmule.db3");

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── Data Layer ───────────────────────────────────────────────────────
        builder.Services.AddSingleton(new DatabaseContext(DatabasePath));

        builder.Services.AddSingleton<IListRepository, ListRepository>();
        builder.Services.AddSingleton<IItemRepository, ItemRepository>();
        builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();

        // ── Service Layer ────────────────────────────────────────────────────
        builder.Services.AddSingleton<IShoppingListService, ShoppingListService>();
        builder.Services.AddSingleton<IShoppingItemService, ShoppingItemService>();
        builder.Services.AddSingleton<ICategoryService, CategoryService>();

        // ── ViewModels ───────────────────────────────────────────────────────
        builder.Services.AddTransient<ListsDashboardViewModel>();
        builder.Services.AddTransient<ListDetailViewModel>();
        builder.Services.AddTransient<AddItemViewModel>();

        // ── Pages ────────────────────────────────────────────────────────────
        builder.Services.AddTransient<ListsDashboardPage>();
        builder.Services.AddTransient<ListDetailPage>();
        builder.Services.AddTransient<AddItemPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
