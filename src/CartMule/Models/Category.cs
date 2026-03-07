using SQLite;

namespace CartMule.Models;

public class Category
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    /// <summary>Controls display order in the list. Lower = higher priority.</summary>
    public int SortOrder { get; set; }

    // Default seeded categories
    public static readonly IReadOnlyList<Category> Defaults =
    [
        new() { Id = 1, Name = "Produce",   SortOrder = 1 },
        new() { Id = 2, Name = "Dairy",     SortOrder = 2 },
        new() { Id = 3, Name = "Bakery",    SortOrder = 3 },
        new() { Id = 4, Name = "Meat",      SortOrder = 4 },
        new() { Id = 5, Name = "Beverages", SortOrder = 5 },
        new() { Id = 6, Name = "Frozen",    SortOrder = 6 },
        new() { Id = 7, Name = "Household", SortOrder = 7 },
        new() { Id = 8, Name = "Other",     SortOrder = 99 },
    ];
}
