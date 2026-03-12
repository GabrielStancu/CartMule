using SQLite;
using TouCart.Models;

namespace TouCart.Data;

public class DatabaseContext
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection? _connection;

    public DatabaseContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null)
            return _connection;

        _connection = new SQLiteAsyncConnection(_dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        await _connection.CreateTableAsync<Category>();
        await _connection.CreateTableAsync<ShoppingList>();
        await _connection.CreateTableAsync<ShoppingItem>();
        await SeedCategoriesAsync(_connection);
        await MigrateAsync(_connection);

        return _connection;
    }

    private static async Task SeedCategoriesAsync(SQLiteAsyncConnection connection)
    {
        var count = await connection.Table<Category>().CountAsync();
        if (count == 0)
            await connection.InsertAllAsync(Category.Defaults);
    }

    private static async Task MigrateAsync(SQLiteAsyncConnection connection)
    {
        // v2: add Shops column to ShoppingList
        try
        {
            await connection.ExecuteAsync(
                "ALTER TABLE ShoppingList ADD COLUMN Shops TEXT NOT NULL DEFAULT ''");
        }
        catch { /* column already exists — safe to ignore */ }

        // v3: per-list categories — add ListId column
        try
        {
            await connection.ExecuteAsync(
                "ALTER TABLE Category ADD COLUMN ListId INTEGER NOT NULL DEFAULT 0");
        }
        catch { /* column already exists */ }

        // Migrate existing lists — copy global categories (ListId=0) to per-list copies
        // and remap items to the new per-list category IDs.
        var lists = await connection.Table<ShoppingList>().ToListAsync();
        foreach (var list in lists)
        {
            var listCatCount = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Category WHERE ListId = ?", list.Id);
            if (listCatCount > 0) continue; // already migrated

            var globalCats = await connection.Table<Category>()
                .Where(c => c.ListId == 0).ToListAsync();

            var idMap = new Dictionary<int, int>();
            foreach (var g in globalCats)
            {
                var copy = new Category { Name = g.Name, SortOrder = g.SortOrder, ListId = list.Id };
                await connection.InsertAsync(copy);
                idMap[g.Id] = copy.Id;
            }

            var items = await connection.Table<ShoppingItem>()
                .Where(i => i.ListId == list.Id).ToListAsync();
            foreach (var item in items)
            {
                if (idMap.TryGetValue(item.CategoryId, out var newCatId))
                {
                    item.CategoryId = newCatId;
                    await connection.UpdateAsync(item);
                }
            }
        }
    }
}
