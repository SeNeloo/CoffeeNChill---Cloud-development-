using Microsoft.AspNetCore.Mvc;
using CoffeeNChill.Gateway.Services;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

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

        public MenuController(FunctionsProxyService proxy)
        {
            _proxy = proxy;
        }

        /// <summary>
        /// Retrieves all menu items.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMenuItems()
        {
            var response = await _proxy.ForwardRequestAsync(
                HttpMethod.Get, "api/menu");

            return await CreateResult(response);
        }
        /// <summary>
        /// Retrieves menu items belonging to a specific category.
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetMenuItemsByCategory(string category)
        {
            var response = await _proxy.ForwardRequestAsync(
                HttpMethod.Get,
                $"api/menu/category/{Uri.EscapeDataString(category)}");

            return await CreateResult(response);
        }
        /// <summary>
        /// Creates a new menu item.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMenuItem(
            [FromBody] JsonElement menuItem)
        {
            var content = new StringContent(
                menuItem.GetRawText(),
                Encoding.UTF8,
                "application/json");

            var response = await _proxy.ForwardRequestAsync(
                HttpMethod.Post, "api/menu", content);

            return await CreateResult(response);
        }
        /// <summary>
        /// Updates an existing menu item.
        /// </summary>
        [HttpPut("category/{category}/{id}")]
        public async Task<IActionResult> UpdateMenuItem(
            string category,
            string id,
            [FromBody] JsonElement menuItem)
        {
            var content = new StringContent(
                menuItem.GetRawText(),
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
        /// </summary>
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

