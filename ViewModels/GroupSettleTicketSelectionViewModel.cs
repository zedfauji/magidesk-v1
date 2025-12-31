using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using System.Collections.ObjectModel;
using System.Linq;

namespace Magidesk.Presentation.ViewModels
{
    public partial class GroupSettleTicketSelectionViewModel : ViewModelBase
    {
        private ObservableCollection<GroupSettleTicketDto> _availableTickets = new();
        private decimal _combinedTotal = 0;
        private int _selectedCount = 0;

        public ObservableCollection<GroupSettleTicketDto> AvailableTickets
        {
            get => _availableTickets;
            set => SetProperty(ref _availableTickets, value);
        }

        public decimal CombinedTotal
        {
            get => _combinedTotal;
            set => SetProperty(ref _combinedTotal, value);
        }

        public int SelectedCount
        {
            get => _selectedCount;
            set => SetProperty(ref _selectedCount, value);
        }

        public GroupSettleTicketSelectionViewModel()
        {
            Title = "Select Tickets for Group Settlement";
            LoadAvailableTickets();
        }

        private void LoadAvailableTickets()
        {
            // For now, create mock data
            // In a real implementation, this would query the backend for open tickets
            var mockTickets = new[]
            {
                new GroupSettleTicketDto 
                { 
                    Id = Guid.NewGuid(), 
                    TicketNumber = "#1001", 
                    GuestCount = 4, 
                    TableNumber = "T1", 
                    TotalAmount = 45.50m,
                    IsSelected = false 
                },
                new GroupSettleTicketDto 
                { 
                    Id = Guid.NewGuid(), 
                    TicketNumber = "#1002", 
                    GuestCount = 2, 
                    TableNumber = "T2", 
                    TotalAmount = 23.25m,
                    IsSelected = false 
                },
                new GroupSettleTicketDto 
                { 
                    Id = Guid.NewGuid(), 
                    TicketNumber = "#1003", 
                    GuestCount = 3, 
                    TableNumber = "T3", 
                    TotalAmount = 67.80m,
                    IsSelected = false 
                }
            };

            foreach (var ticket in mockTickets)
            {
                ticket.SelectionChanged += RefreshCombinedTotal;
                AvailableTickets.Add(ticket);
            }

            UpdateCombinedTotal();
        }

        private void UpdateCombinedTotal()
        {
            CombinedTotal = AvailableTickets
                .Where(t => t.IsSelected)
                .Sum(t => t.TotalAmount);
            
            SelectedCount = AvailableTickets.Count(t => t.IsSelected);
        }

        public void RefreshCombinedTotal()
        {
            UpdateCombinedTotal();
        }
    }

    public class GroupSettleTicketDto : ObservableObject
    {
        private bool _isSelected;
        private string _ticketNumber;
        private int _guestCount;
        private string _tableNumber;
        private decimal _totalAmount;

        public Guid Id { get; set; }
        
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    // Notify parent ViewModel of selection change
                    // This will be handled by the parent ViewModel
                    SelectionChanged?.Invoke();
                }
            }
        }

        public event Action? SelectionChanged;

        public string TicketNumber
        {
            get => _ticketNumber;
            set => SetProperty(ref _ticketNumber, value);
        }

        public int GuestCount
        {
            get => _guestCount;
            set => SetProperty(ref _guestCount, value);
        }

        public string TableNumber
        {
            get => _tableNumber;
            set => SetProperty(ref _tableNumber, value);
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }
    }
}
