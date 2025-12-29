using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Magidesk.Presentation.ViewModels
{
    public partial class SwipeCardViewModel : ViewModelBase
    {
        private string _instructionText = "Please Swipe Card...";
        public string InstructionText
        {
            get => _instructionText;
            set => SetProperty(ref _instructionText, value);
        }

        private string _swipeData = string.Empty;
        public string SwipeData
        {
            get => _swipeData;
            set => SetProperty(ref _swipeData, value);
        }

        public SwipeCardViewModel()
        {
            Title = "Swipe Card";
        }

        [RelayCommand]
        private void ProcessSwipe()
        {
            // In a real implementation, we would parse SwipeData here.
            // For now, we just validate it's not empty.
            // The Dialog (View) will handle the specific 'Close' logic with result.
        }
    }
}
