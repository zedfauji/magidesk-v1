using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using MediatR;

namespace Magidesk.Presentation.ViewModels;

public sealed partial class MemberCheckInViewModel : ViewModelBase
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;

    private Member? _selectedMember;
    public Member? SelectedMember
    {
        get => _selectedMember;
        set
        {
            if (SetProperty(ref _selectedMember, value))
            {
                OnPropertyChanged(nameof(HasSelectedMember));
                OnPropertyChanged(nameof(CanCheckIn));
            }
        }
    }

    public bool HasSelectedMember => SelectedMember != null;
    public bool CanCheckIn => SelectedMember != null && SelectedMember.IsActive;

    public ICommand SearchCommand { get; }
    public ICommand CheckInCommand { get; }

    private string _searchTerm = string.Empty;
    public string SearchTerm { get => _searchTerm; set => SetProperty(ref _searchTerm, value); }

    public MemberCheckInViewModel(
        IMemberRepository memberRepository,
        IMediator mediator,
        IDialogService dialogService)
    {
        _memberRepository = memberRepository;
        _mediator = mediator;
        _dialogService = dialogService;

        SearchCommand = new AsyncRelayCommand(SearchAsync);
        CheckInCommand = new AsyncRelayCommand(CheckInAsync, () => CanCheckIn);
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm)) return;

        IsBusy = true;
        try
        {
            // Search by member number
            var member = await _memberRepository.GetByMemberNumberAsync(SearchTerm);
            if (member == null)
            {
                // Fallback: search by customer name in a real implementation
                await _dialogService.ShowMessageAsync("Not Found", "No member found with that number.");
                return;
            }
            SelectedMember = member;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Search Failed", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CheckInAsync()
    {
        if (SelectedMember == null) return;

        IsBusy = true;
        try
        {
            var command = new CheckInMemberCommand { MemberId = SelectedMember.Id };
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                await _dialogService.ShowMessageAsync("Success", result.Message);
                // In a real app, close the dialog here
            }
            else
            {
                await _dialogService.ShowErrorAsync("Check-In Failed", result.Message);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
