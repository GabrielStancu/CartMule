using SQLite;
using CartMule.Models;

namespace CartMule.Data;

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

        return _connection;
    }

    private static async Task SeedCategoriesAsync(SQLiteAsyncConnection connection)
    {
        var count = await connection.Table<Category>().CountAsync();
        if (count == 0)
            await connection.InsertAllAsync(Category.Defaults);
    }
}
