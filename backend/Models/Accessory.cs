namespace Demo.Api.Models;

/// <summary>
/// Represents a pet accessory product in the pet store.
/// </summary>
public class Accessory
{
    /// <summary>
    /// Unique identifier for the accessory.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the accessory.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description of the accessory.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Category of the accessory (e.g., toys, bedding, grooming, etc.).
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Price of the accessory.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Quantity in stock.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// URL to the product image.
    /// </summary>
    public string? ImageUrl { get; set; }
}
