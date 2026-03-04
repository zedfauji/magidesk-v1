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

using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for product management and filtering operations.
/// Handles filtering and display of products based on category, subcategory, and search.
/// </summary>
public partial class OrderPageViewModel
{
    private void RecalculateTotals()
    {
        Subtotal = OrderItems.Sum(item => item.LineTotal);
        TaxAmount = Subtotal * TaxRate;
        Total = Subtotal + TaxAmount;

        OnPropertyChanged(nameof(TotalItemCount));

        _logger.LogDebug("Recalculated totals: Subtotal={Subtotal}, Tax={Tax}, Total={Total}",
            Subtotal, TaxAmount, Total);
    }

    private void FilterProducts()
    {
        try
        {
            _logger.LogInformation("FilterProducts called - SelectedCategory: {Category}, SelectedSubcategory: {Subcategory}, SearchQuery: {Search}, TotalProducts: {Total}",
                SelectedCategory?.Name ?? "null", SelectedSubcategory ?? "null", SearchQuery ?? "null", _allProducts.Count);

            FilteredProducts.Clear();

            var query = _allProducts.AsEnumerable();

            // Filter by search query (name or SKU, case-insensitive)
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.SKU.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                );
                _logger.LogInformation("After search filter: {Count} products", query.Count());
            }

            // Filter by category
            if (SelectedCategory != null && SelectedCategory.Name != "Popular")
            {
                var beforeCount = query.Count();
                query = query.Where(p => p.CategoryName.Equals(SelectedCategory.Name, StringComparison.OrdinalIgnoreCase));
                _logger.LogInformation("Category filter '{Category}': {Before} -> {After} products",
                    SelectedCategory.Name, beforeCount, query.Count());
            }

            // Filter by subcategory
            if (!string.IsNullOrWhiteSpace(SelectedSubcategory))
            {
                var beforeCount = query.Count();
                query = query.Where(p => p.SubcategoryName.Equals(SelectedSubcategory, StringComparison.OrdinalIgnoreCase));
                _logger.LogInformation("Subcategory filter '{Subcategory}': {Before} -> {After} products",
                    SelectedSubcategory, beforeCount, query.Count());
            }

            // Apply filtered results
            foreach (var product in query)
            {
                FilteredProducts.Add(product);
            }

            _logger.LogInformation("FilterProducts completed: {Count} products in FilteredProducts", FilteredProducts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to filter products");
        }
    }

    private void OnSearchProduct()
    {
        FilterProducts();
    }

    private void OnSelectCategory(ProductCategoryViewModel? category)
    {
        SelectedCategory = category;
        SelectedSubcategory = null; // Clear subcategory when changing category
        Subcategories.Clear();

        if (category != null)
        {
            _logger.LogInformation("Selected category: {CategoryName}", category.Name);

            // Load subcategories for the selected category
            var subcats = _allProducts
                .Where(p => p.CategoryName.Equals(category.Name, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.SubcategoryName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(s => !string.IsNullOrEmpty(s))
                .OrderBy(s => s)
                .ToList();

            foreach (var subcat in subcats)
            {
                Subcategories.Add(subcat);
            }

            _logger.LogInformation("Loaded {Count} subcategories for category {CategoryName}", Subcategories.Count, category.Name);
        }

        FilterProducts();
    }

    private void OnSelectSubcategory(string? subcategory)
    {
        SelectedSubcategory = subcategory;
        if (subcategory != null)
        {
            _logger.LogInformation("Selected subcategory: {SubcategoryName}", subcategory);
        }

        FilterProducts();
    }
}
