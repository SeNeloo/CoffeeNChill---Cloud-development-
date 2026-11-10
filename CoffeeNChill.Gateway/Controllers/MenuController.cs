using Microsoft.AspNetCore.Mvc;
using CoffeeNChill.Gateway.Services;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using CoffeeNChill.Gateway.DTOs;
using CoffeeNChill.Models;
using System.Net.Http.Json;
using System.Linq;
using System.Collections.Generic;

namespace CoffeeNChill.Gateway.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing coffee shop menu items.
    /// </summary>
    [ApiController]
    [Route("api/menu")]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly FunctionsProxyService _proxy;
        private readonly MenuItemMappingService _mappingService;

        public MenuController(
            FunctionsProxyService proxy,
            MenuItemMappingService mappingService)
        {
            _proxy = proxy;
            _mappingService = mappingService;
        }

        /// <summary>
        /// Retrieves all menu items.
        /// Accessible to all authenticated users.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAllMenuItems()
        {
            var response = await _proxy.ForwardRequestAsync(
                HttpMethod.Get, "api/menu");

            if (!response.IsSuccessStatusCode)
            {
                return await CreateResult(response);
            }

            var items = await response.Content.ReadFromJsonAsync<List<MenuItem>>();

            if (items == null)
            {
                return Ok(new List<MenuItemDto>());
            }

            var menuDtos = items
                .Select(item => _mappingService.MapToDto(item))
                .ToList();

            return Ok(menuDtos);
        }

        /// <summary>
        /// Retrieves menu items belonging to a specific category.
        /// Accessible to all authenticated users.
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetMenuItemsByCategory(string category)
        {
            var response = await _proxy.ForwardRequestAsync(
                HttpMethod.Get,
                $"api/menu/category/{Uri.EscapeDataString(category)}");

            if (!response.IsSuccessStatusCode)
            {
                return await CreateResult(response);
            }

            var items = await response.Content.ReadFromJsonAsync<List<MenuItem>>();

            if (items == null)
            {
                return Ok(new List<MenuItemDto>());
            }

            var menuDtos = items
                .Select(item => _mappingService.MapToDto(item))
                .ToList();

            return Ok(menuDtos);
        }

        /// <summary>
        /// Creates a new menu item.
        /// Restricted to Admin users.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateMenuItem(
            [FromBody] MenuItemDto menuItem)
        {
            var entity = _mappingService.MapToEntity(menuItem);

            var content = new StringContent(
                JsonSerializer.Serialize(entity),
                Encoding.UTF8,
                "application/json");

            var response = await _proxy.ForwardRequestAsync(
                HttpMethod.Post, "api/menu", content);

            return await CreateResult(response);
        }

        /// <summary>
        /// Updates an existing menu item.
        /// Restricted to Admin users.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("category/{category}/{id}")]
        public async Task<IActionResult> UpdateMenuItem(
            string category,
            string id,
            [FromBody] MenuItemDto menuItem)
        {
            menuItem.Category = category;
            menuItem.Id = id;

            var entity = _mappingService.MapToEntity(menuItem);

            var content = new StringContent(
                JsonSerializer.Serialize(entity),
                Encoding.UTF8,
                "application/json");

            var response = await _proxy.ForwardRequestAsync(
                HttpMethod.Put,
                $"api/menu/{Uri.EscapeDataString(category)}/{Uri.EscapeDataString(id)}",
                content);

            return await CreateResult(response);
        }

        /// <summary>
        /// Deletes a menu item.
        /// Restricted to Admin users.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("category/{category}/{id}")]
        public async Task<IActionResult> DeleteMenuItem(
            string category,
            string id)
        {
            var response = await _proxy.ForwardRequestAsync(
                HttpMethod.Delete,
                $"api/menu/{Uri.EscapeDataString(category)}/{Uri.EscapeDataString(id)}");

            return await CreateResult(response);
        }

        private async Task<IActionResult> CreateResult(
            HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                Content = content,
                ContentType = response.Content.Headers.ContentType?.ToString()
                    ?? "application/json"
            };
        }
    }
}