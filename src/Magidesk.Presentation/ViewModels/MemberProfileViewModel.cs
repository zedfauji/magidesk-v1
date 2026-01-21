using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Presentation.ViewModels;

public sealed partial class MemberProfileViewModel : ViewModelBase
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMembershipTierRepository _tierRepository;
    private readonly IDialogService _dialogService;

    private Member? _member;
    public Member? Member
    {
        get => _member;
        set
        {
            if (SetProperty(ref _member, value))
            {
                OnPropertyChanged(nameof(HasMember));
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(MemberNumber));
                OnPropertyChanged(nameof(TierName));
                OnPropertyChanged(nameof(StatusDisplay));
                OnPropertyChanged(nameof(PrepaidBalance));
                OnPropertyChanged(nameof(IsActive));
            }
        }
    }

    public bool HasMember => Member != null;
    public string FullName => Member?.Customer?.FullName ?? "Unknown Member";
    public string MemberNumber => Member?.MemberNumber ?? "N/A";
    public string TierName => Member?.Tier?.Name ?? "No Tier";
    public string StatusDisplay => Member?.Status.ToString() ?? "Unknown";
    public string PrepaidBalance => Member?.PrepaidBalance.ToString() ?? "$0.00";
    public bool IsActive => Member?.IsActive ?? false;

    public ICommand LoadMemberCommand { get; }
    public ICommand CheckInCommand { get; }
    public ICommand AddCreditCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand RenewCommand { get; }

    public MemberProfileViewModel(
        IMemberRepository memberRepository,
        IMembershipTierRepository tierRepository,
        IDialogService dialogService)
    {
        Title = "Member Profile";
        _memberRepository = memberRepository;
        _tierRepository = tierRepository;
        _dialogService = dialogService;

        LoadMemberCommand = new AsyncRelayCommand<Guid>(LoadMemberAsync);
        CheckInCommand = new AsyncRelayCommand(CheckInAsync);
        AddCreditCommand = new AsyncRelayCommand(AddCreditAsync);
        EditCommand = new RelayCommand(Edit);
        RenewCommand = new AsyncRelayCommand(RenewAsync);
    }

    private async Task LoadMemberAsync(Guid customerId)
    {
        IsBusy = true;
        try
        {
            var member = await _memberRepository.GetByCustomerIdAsync(customerId);
            if (member == null)
            {
                // This customer is not a member yet. 
                // In a real app, we might prompt to create a membership.
                await _dialogService.ShowMessageAsync("Not a Member", "This customer does not have an active membership.");
                return;
            }
            Member = member;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Load Failed", $"Error loading member profile: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CheckInAsync()
    {
        if (Member == null) return;
        
        // This would typically log a visit or open a check-in dialog.
        // For now, we'll just show a message.
        await _dialogService.ShowMessageAsync("Check In", $"{FullName} has been checked in.");
    }

    private async Task AddCreditAsync()
    {
        if (Member == null) return;

        // In a real app, prompt for amount.
        // For demonstration, we'll just show the action.
        await _dialogService.ShowMessageAsync("Add Credit", "Prepaid credit management will be implemented in F.11.");
    }

    private void Edit()
    {
        // Navigate to edit page or open dialog.
    }

    private async Task RenewAsync()
    {
        if (Member == null) return;

        try
        {
            // Prompt for renewal duration or use default.
            var newExpiration = DateTime.UtcNow.AddYears(1);
            Member.Renew(newExpiration);
            await _memberRepository.UpdateAsync(Member);
            
            OnPropertyChanged(nameof(StatusDisplay));
            OnPropertyChanged(nameof(IsActive));
            
            await _dialogService.ShowMessageAsync("Membership Renewed", $"Membership has been renewed until {newExpiration:d}");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Renewal Failed", ex.Message);
        }
    }
}
