using SQLite;

namespace TouCart.Models;

public class ShoppingItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>Foreign key to ShoppingList.Id.</summary>
    [NotNull, Indexed]
    public int ListId { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    /// <summary>Foreign key to Category.Id. Defaults to "Other" (8).</summary>
    public int CategoryId { get; set; } = 8;

    /// <summary>Free-text quantity, e.g. "2 kg", "1 pack", "500g".</summary>
    public string Quantity { get; set; } = string.Empty;

    /// <summary>True when the item has been placed in the cart.</summary>
    public bool IsBought { get; set; }

    /// <summary>Manual sort order within the same category+bought group.</summary>
    public int SortOrder { get; set; }
}
