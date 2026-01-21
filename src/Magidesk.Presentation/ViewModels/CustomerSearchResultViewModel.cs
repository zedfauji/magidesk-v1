using CommunityToolkit.Mvvm.ComponentModel;
using Magidesk.Application.DTOs;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for a single customer search result.
/// </summary>
public partial class CustomerSearchResultViewModel : ObservableObject
{
    [ObservableProperty]
    private CustomerSearchResultDto _dto;

    public string FullName => Dto.FullName;
    public string Phone => Dto.Phone;
    public string Email => Dto.Email ?? string.Empty;
    public bool IsMember => Dto.IsMember;
    public string TierName => Dto.MembershipTier ?? "None";
    
    // Tier Colors (Placeholder until F.3/F.4)
    public string TierColor => Dto.MembershipTier switch
    {
        "Gold" => "#FFD700",
        "Silver" => "#C0C0C0",
        "Bronze" => "#CD7F32",
        _ => "#808080"
    };

    public CustomerSearchResultViewModel(CustomerSearchResultDto dto)
    {
        _dto = dto;
    }
}
