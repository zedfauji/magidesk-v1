using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;
using Magidesk.Domain.Enumerations;
using System;

namespace Magidesk.Presentation.Views.Components;

public sealed partial class TableShapePalette : UserControl
{
    public TableDesignerViewModel? ViewModel { get; set; }

    public TableShapePalette()
    {
        this.InitializeComponent();
    }

    private void ShapeRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is RadioButton radioButton && radioButton.Tag is string shapeString)
            {
                if (Enum.TryParse<TableShapeType>(shapeString, out var shape))
                {
                    if (ViewModel != null)
                    {
                        ViewModel.SelectedShape = shape;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ShapeRadioButton_Checked: {ex.Message}");
        }
    }
}
