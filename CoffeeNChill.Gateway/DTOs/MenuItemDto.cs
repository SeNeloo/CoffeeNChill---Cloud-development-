namespace CoffeeNChill.Gateway.DTOs;

/// <summary>
/// Represents menu item data transferred through the API Gateway.
/// </summary>
public class MenuItemDto
{
    /// <summary>
    /// Gets or sets the menu item's identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the menu item's category.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the menu item's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the menu item's description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the menu item's price.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Gets or sets whether the menu item is available.
    /// </summary>
    public bool IsAvailable { get; set; }
}
