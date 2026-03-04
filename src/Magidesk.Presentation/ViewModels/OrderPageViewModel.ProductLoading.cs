using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for product loading operations.
/// Handles loading and filtering of menu items from the catalog.
/// </summary>
public partial class OrderPageViewModel
{
    private async Task LoadProductsAsync()
    {
        try
        {
            _logger.LogInformation("LoadProductsAsync starting");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getMenuItemsHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetMenuItemsQuery, List<MenuItemDto>>>();
                var menuRepository = scope.ServiceProvider.GetRequiredService<IMenuRepository>();

                _logger.LogInformation("Calling GetMenuItemsQuery");
                var menuItems = await getMenuItemsHandler.HandleAsync(new GetMenuItemsQuery { IsActive = true });
                _logger.LogInformation("GetMenuItemsQuery returned {Count} items", menuItems?.Count ?? 0);

                _allProducts.Clear();
                foreach (var item in menuItems)
                {
                    // Get the full menu item to check for modifiers and get group/category info
                    var menuItem = await menuRepository.GetByIdAsync(item.Id);
                    bool hasModifiers = menuItem?.ModifierGroups.Any() ?? false;

                    // Get category and group (subcategory) names
                    string categoryName = menuItem?.Category?.Name ?? item.CategoryName ?? "Uncategorized";
                    string groupName = menuItem?.Group?.Name ?? string.Empty;

                    _allProducts.Add(new ProductViewModel
                    {
                        ProductId = item.Id,
                        Name = item.Name,
                        SKU = item.Id.ToString().Substring(0, 8), // Use first 8 chars of GUID as SKU
                        Price = item.Price,
                        CategoryName = categoryName,
                        SubcategoryName = groupName, // Group is the subcategory
                        HasModifiers = hasModifiers,
                        IsAvailable = item.IsActive
                    });
                }

                // Apply initial filter
                FilterProducts();

                _logger.LogInformation("Loaded {Count} products, filtered to {FilteredCount}", _allProducts.Count, FilteredProducts.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load products");
            await _dialogService.ShowErrorAsync(
                "Error Loading Products",
                $"Failed to load menu items:\n\n{ex.Message}",
                ex.ToString());
        }
    }
}
