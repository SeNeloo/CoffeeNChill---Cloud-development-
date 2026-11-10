using CoffeeNChill.Gateway.DTOs;
using CoffeeNChill.Models;

namespace CoffeeNChill.Gateway.Services;

/// <summary>
/// Maps menu data between API DTOs and storage entities.
/// </summary>
public class MenuItemMappingService
{
    public MenuItemDto MapToDto(MenuItem item)
    {
        return new MenuItemDto
        {
            Id = item.RowKey,
            Category = item.PartitionKey,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            IsAvailable = item.IsAvailable
        };
    }

    public MenuItem MapToEntity(MenuItemDto dto)
    {
        return new MenuItem
        {
            PartitionKey = dto.Category,
            RowKey = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IsAvailable = dto.IsAvailable
        };
    }
}