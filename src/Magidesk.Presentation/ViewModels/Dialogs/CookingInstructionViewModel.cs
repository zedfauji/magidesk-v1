using System.Collections.ObjectModel;
using System.Windows.Input;
using Magidesk.ViewModels;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public class CookingInstructionViewModel : ViewModelBase
{
    private string _selectedInstructions = string.Empty;
    public string SelectedInstructions
    {
        get => _selectedInstructions;
        set => SetProperty(ref _selectedInstructions, value);
    }

    public ObservableCollection<string> PredefinedInstructions { get; } = new();

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand AddInstructionCommand { get; }

    public System.Action? CloseAction { get; set; }
    public System.Action? CancelAction { get; set; }
    
    public bool IsConfirmed { get; private set; }

    public CookingInstructionViewModel(string currentInstructions = "")
    {
        Title = "Cooking Instructions";
        SelectedInstructions = currentInstructions;

        // F-0036: Pre-defined options per audit
        PredefinedInstructions.Add("Rare");
        PredefinedInstructions.Add("Medium Rare");
        PredefinedInstructions.Add("Medium");
        PredefinedInstructions.Add("Medium Well");
        PredefinedInstructions.Add("Well Done");
        PredefinedInstructions.Add("No Onion");
        PredefinedInstructions.Add("No Garlic");
        PredefinedInstructions.Add("Extra Spicy");
        PredefinedInstructions.Add("Sauce on Side");
        PredefinedInstructions.Add("Allergy: Gluten");
        PredefinedInstructions.Add("Allergy: Dairy");
        PredefinedInstructions.Add("RUSH");

        ConfirmCommand = new RelayCommand(() => { IsConfirmed = true; CloseAction?.Invoke(); });
        CancelCommand = new RelayCommand(() => { IsConfirmed = false; CancelAction?.Invoke(); });
        
        AddInstructionCommand = new RelayCommand<string>(AddInstruction);
    }

    private void AddInstruction(string instruction)
    {
        if (string.IsNullOrWhiteSpace(SelectedInstructions))
        {
            SelectedInstructions = instruction;
        }
        else
        {
            // Avoid duplicates? Audit doesn't specify. Append with comma.
            if (!SelectedInstructions.Contains(instruction))
            {
                 SelectedInstructions += $", {instruction}";
            }
        }
    }
}
