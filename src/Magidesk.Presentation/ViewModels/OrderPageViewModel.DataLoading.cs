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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.Views.Dialogs;
using System.Collections.ObjectModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for data loading operations.
/// Handles loading of tickets, tables, categories, and products.
/// </summary>
public partial class OrderPageViewModel
{
    private async Task LoadTicketAsync()
    {
        if (!_ticketId.HasValue) return;

        try
        {
            IsBusy = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getTicketHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTicketQuery, TicketDto?>>();
                _ticket = await getTicketHandler.HandleAsync(new GetTicketQuery { TicketId = _ticketId.Value });

                if (_ticket != null)
                {
                    // Load table information from ticket if available
                    if (_ticket.TableNumbers != null && _ticket.TableNumbers.Any() && !_tableId.HasValue)
                    {
                        // Get the first table number from the ticket
                        var tableNumber = _ticket.TableNumbers.First();
                        _logger.LogInformation("Ticket has table number {TableNumber}, loading table details", tableNumber);

                        // Get the table ID from the repository using the table number
                        var tableRepository = scope.ServiceProvider.GetRequiredService<ITableRepository>();
                        var tables = await tableRepository.GetAllAsync();
                        var table = tables.FirstOrDefault(t => t.TableNumber == tableNumber);

                        if (table != null)
                        {
                            _tableId = table.Id;
                            TableNumber = $"TABLE {table.TableNumber}";
                            _logger.LogInformation("Loaded table ID {TableId} (Table {TableNumber}) from ticket", _tableId, table.TableNumber);
                        }
                        else
                        {
                            _logger.LogWarning("Table with number {TableNumber} not found in repository", tableNumber);
                            TableNumber = $"TABLE {tableNumber}";
                        }
                    }
                    else if (!string.IsNullOrEmpty(_ticket.TableName) && !_tableId.HasValue)
                    {
                        // Fallback: use table name if available
                        TableNumber = _ticket.TableName;
                        _logger.LogInformation("Using table name from ticket: {TableName}", _ticket.TableName);
                    }

                    // Load order items
                    OrderItems.Clear();
                    foreach (var line in _ticket.OrderLines)
                    {
                        OrderItems.Add(new OrderItemViewModel
                        {
                            OrderItemId = line.Id,
                            ProductName = line.MenuItemName,
                            Quantity = (int)line.Quantity,
                            UnitPrice = line.UnitPrice,
                            LineTotal = line.TotalAmount,
                            Modifiers = new ObservableCollection<string>(
                                line.Modifiers?.Select(m => m.Name) ?? Enumerable.Empty<string>()
                            )
                        });
                    }

                    RecalculateTotals();

                    // Notify property changes
                    OnPropertyChanged(nameof(TicketNumber));
                    OnPropertyChanged(nameof(TicketStartTime));
                    OnPropertyChanged(nameof(WaitTime));
                    OnPropertyChanged(nameof(TotalItemCount));

                    // Session state properties
                    OnPropertyChanged(nameof(CurrentSessionState));
                    OnPropertyChanged(nameof(IsSessionActive));
                    OnPropertyChanged(nameof(IsSessionPaused));
                    OnPropertyChanged(nameof(SessionButtonText));
                    OnPropertyChanged(nameof(IsEndSessionEnabled));
                    OnPropertyChanged(nameof(SessionDurationDisplay));

                    _logger.LogInformation("Loaded ticket {TicketId} with {ItemCount} items", _ticketId, OrderItems.Count);
                }
                else
                {
                    _logger.LogWarning("Ticket {TicketId} not found", _ticketId);
                    await _dialogService.ShowWarningAsync(
                        "Ticket Not Found",
                        $"Ticket {_ticketId} could not be found. It may have been deleted or moved.");
                }
            }
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while loading ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Error Loading Ticket",
                $"An error occurred while loading the ticket:\n\n{ex.Message}",
                ex.ToString());
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadTableAsync()
    {
        if (!_tableId.HasValue) return;

        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getTableHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTableQuery, GetTableResult>>();
                var result = await getTableHandler.HandleAsync(new GetTableQuery { TableId = _tableId.Value });

                if (result?.Table != null)
                {
                    TableNumber = $"TABLE {result.Table.TableNumber} (GUESTS: {GuestCount})";
                    _logger.LogInformation("Loaded table {TableNumber}", result.Table.TableNumber);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load table {TableId}", _tableId);
        }
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            _logger.LogInformation("LoadCategoriesAsync starting");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var menuCategoryRepository = scope.ServiceProvider.GetRequiredService<IMenuCategoryRepository>();

                var dbCategories = await menuCategoryRepository.GetAllAsync();
                _logger.LogInformation("Loaded {Count} categories from database", dbCategories?.Count() ?? 0);

                Categories.Clear();

                // Add "Popular" as first category (special category that shows all)
                Categories.Add(new ProductCategoryViewModel { Name = "Popular", IconName = "\uE734" }); // FavoriteStar

                // Add categories from database
                if (dbCategories != null)
                {
                    _logger.LogInformation("Adding {Count} categories from database:", dbCategories.Count());
                    foreach (var category in dbCategories.Where(c => c.IsActive).OrderBy(c => c.SortOrder))
                    {
                        _logger.LogInformation("  Category from DB: '{Name}'", category.Name);
                        Categories.Add(new ProductCategoryViewModel
                        {
                            Name = category.Name,
                            IconName = GetIconForCategory(category.Name)
                        });
                    }
                }

                // Fallback: if no categories in database, add default ones
                if (Categories.Count == 1) // Only "Popular"
                {
                    _logger.LogWarning("No categories found in database, using defaults");
                    Categories.Add(new ProductCategoryViewModel { Name = "Food", IconName = "\uE787" }); // Restaurant
                    Categories.Add(new ProductCategoryViewModel { Name = "Drinks", IconName = "\uE8C4" }); // Coffee
                    Categories.Add(new ProductCategoryViewModel { Name = "Desserts", IconName = "\uE7E3" }); // Cake
                    Categories.Add(new ProductCategoryViewModel { Name = "Sides", IconName = "\uE7E8" }); // Food
                    Categories.Add(new ProductCategoryViewModel { Name = "Retail", IconName = "\uE719" }); // ShoppingCart
                }

                // Select first category by default (Popular)
                if (Categories.Any())
                {
                    SelectedCategory = Categories.First();
                    _logger.LogInformation("Selected default category: {CategoryName}", SelectedCategory.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories");

            // Fallback to default categories on error
            Categories.Clear();
            Categories.Add(new ProductCategoryViewModel { Name = "Popular", IconName = "\uE734" }); // FavoriteStar
            Categories.Add(new ProductCategoryViewModel { Name = "All Items", IconName = "\uE787" }); // Restaurant

            if (Categories.Any())
            {
                SelectedCategory = Categories.First();
            }
        }
    }

    private string GetIconForCategory(string categoryName)
    {
        // Map category names to Segoe MDL2 Assets icon glyphs (Unicode characters)
        var lowerName = categoryName.ToLowerInvariant();

        // Popular/Star
        if (lowerName.Contains("popular"))
            return "\uE734"; // FavoriteStar
        // Food/Meal
        if (lowerName.Contains("food") || lowerName.Contains("meal") || lowerName.Contains("អាហារ") || lowerName.Contains("ម្ហូប"))
            return "\uE787"; // Restaurant
        // Drinks/Beverages
        if (lowerName.Contains("drink") || lowerName.Contains("beverage") || lowerName.Contains("ភេសជ្ជៈ"))
            return "\uE8C4"; // Drink (Coffee)
        // Desserts
        if (lowerName.Contains("dessert") || lowerName.Contains("sweet") || lowerName.Contains("បង្អែម"))
            return "\uE7E3"; // Cake
        // Appetizers/Starters
        if (lowerName.Contains("appetizer") || lowerName.Contains("starter"))
            return "\uE7E8"; // Food
        // Sides
        if (lowerName.Contains("side"))
            return "\uE7E8"; // Food
        // Burgers
        if (lowerName.Contains("burger"))
            return "\uE7E8"; // Food
        // Pizza
        if (lowerName.Contains("pizza"))
            return "\uE7E8"; // Food
        // Salads
        if (lowerName.Contains("salad"))
            return "\uE7E8"; // Food
        // Combos
        if (lowerName.Contains("combo"))
            return "\uE7E8"; // Food
        // Retail/Merchandise
        if (lowerName.Contains("retail") || lowerName.Contains("merchandise"))
            return "\uE719"; // ShoppingCart
        // Misc
        if (lowerName.Contains("misc"))
            return "\uE8FD"; // More

        return "\uE787"; // Default: Restaurant icon
    }

}
