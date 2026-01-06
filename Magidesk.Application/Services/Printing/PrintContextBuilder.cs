using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs.Printing;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services.Printing;



public class PrintContextBuilder : IPrintContextBuilder
{
    private readonly IUserRepository _userRepository;
    private readonly IRestaurantConfigurationRepository _restaurantConfigRepository;

    public PrintContextBuilder(
        IUserRepository userRepository,
        IRestaurantConfigurationRepository restaurantConfigRepository)
    {
        _userRepository = userRepository;
        _restaurantConfigRepository = restaurantConfigRepository;
    }

    public async Task<TicketPrintModel> BuildTicketContextAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        var model = new TicketPrintModel();

        // 1. Basic Info
        model.TicketNumber = ticket.TicketNumber.ToString();
        model.Date = DateTime.Now.ToString("MM/dd/yyyy");
        model.Time = DateTime.Now.ToString("HH:mm");
        model.TableName = ticket.TableNumbers.Any() ? string.Join(",", ticket.TableNumbers) : "No Table";

        // 2. Resolve Server Name
        if (ticket.CreatedBy != null)
        {
            var user = await _userRepository.GetByIdAsync(ticket.CreatedBy.Value, cancellationToken);
            model.ServerName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Unknown";
        }
        else
        {
            model.ServerName = "System";
        }

        // 3. Financials
        model.Subtotal = ticket.SubtotalAmount.Amount.ToString("C2");
        model.Tax = ticket.TaxAmount.Amount.ToString("C2");
        model.Total = ticket.TotalAmount.Amount.ToString("C2");
        model.BalanceDue = ticket.DueAmount.Amount.ToString("C2");

        // 4. Restaurant Info (Hardcoded fallback if config missing, mimicking current state but ready for generic config)
        var config = await _restaurantConfigRepository.GetConfigurationAsync(cancellationToken);
        if (config != null)
        {
            model.Restaurant.Name = config.Name;
            model.Restaurant.Address = config.Address;
            model.Restaurant.Phone = config.Phone;
        }
        else
        {
            // Fallback to match existing hardcoded strings if DB is empty
            model.Restaurant.Name = "MAGIDESK POS";
            model.Restaurant.Address = "123 Main Street, Cityville";
            model.Restaurant.Phone = "(555) 123-4567";
        }

        // 5. Lines
        foreach (var line in ticket.OrderLines)
        {
            var lineModel = new OrderLinePrintModel
            {
                Quantity = line.Quantity,
                Name = line.MenuItemName,
                Price = line.UnitPrice.Amount.ToString("F2"), // Just amount for unit price
                Total = line.TotalAmount.Amount.ToString("F2"),
                Instructions = line.Instructions ?? string.Empty
            };

            foreach (var mod in line.Modifiers)
            {
                lineModel.Modifiers.Add($"{mod.Name} (+{mod.TotalAmount.Amount:F2})");
            }

            model.Lines.Add(lineModel);
        }

        // 6. Payments
        foreach (var pay in ticket.Payments)
        {
            model.Payments.Add(new PaymentPrintModel
            {
                Type = pay.PaymentType.ToString(),
                Amount = pay.Amount.Amount.ToString("C2")
            });
        }

        return model;
    }

    public async Task<TicketPrintModel> BuildKitchenContextAsync(Ticket ticket, IEnumerable<OrderLine> lines, CancellationToken cancellationToken)
    {
        var model = new TicketPrintModel();

        // 1. Basic Info
        model.TicketNumber = ticket.TicketNumber.ToString();
        model.Date = DateTime.Now.ToString("MM/dd/yyyy");
        model.Time = DateTime.Now.ToString("HH:mm");
        model.TableName = ticket.TableNumbers.Any() ? string.Join(",", ticket.TableNumbers) : "No Table";

        // 2. Resolve Server Name
        if (ticket.CreatedBy != null)
        {
            var user = await _userRepository.GetByIdAsync(ticket.CreatedBy.Value, cancellationToken);
            model.ServerName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Unknown";
        }
        else
        {
            model.ServerName = "System";
        }

        // 3. Financials (Kitchen tickets usually don't show prices, but we can leave them empty or 0)
        model.Subtotal = ""; 
        model.Tax = "";
        model.Total = "";
        model.BalanceDue = "";

        // 4. Restaurant Info (Minimal needed for kitchen, but good to have)
        var config = await _restaurantConfigRepository.GetConfigurationAsync(cancellationToken);
        if (config != null)
        {
            model.Restaurant.Name = config.Name;
        }
        else
        {
            model.Restaurant.Name = "MAGIDESK POS";
        }

        // 5. Lines (ONLY the specific lines for this station)
        foreach (var line in lines)
        {
            var lineModel = new OrderLinePrintModel
            {
                Quantity = line.Quantity,
                Name = line.MenuItemName,
                Price = "", // Not needed for kitchen
                Total = "",
                Instructions = line.Instructions ?? string.Empty
            };

            foreach (var mod in line.Modifiers)
            {
                lineModel.Modifiers.Add($"{mod.Name}"); // Just name, no price
            }

            model.Lines.Add(lineModel);
        }

        // 6. Payments (Not needed for kitchen)
        
        return model;
    }
}

