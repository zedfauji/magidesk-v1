using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.ViewModels
{
    public partial class AuthorizationCodeViewModel : ViewModelBase
    {
        private string _authCode = string.Empty;
        public string AuthCode
        {
            get => _authCode;
            set => SetProperty(ref _authCode, value);
        }

        private string _selectedCardType = "Visa";
        public string SelectedCardType
        {
            get => _selectedCardType;
            set => SetProperty(ref _selectedCardType, value);
        }

        public AuthorizationCodeViewModel()
        {
            Title = "Authorization Code Entry";
        }
    }
}
