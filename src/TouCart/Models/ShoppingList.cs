using SQLite;

namespace TouCart.Models;

public class ShoppingList
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Updated whenever any item in this list changes.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Optional comma/semicolon separated shops where this list applies.</summary>
    public string Shops { get; set; } = string.Empty;
}
