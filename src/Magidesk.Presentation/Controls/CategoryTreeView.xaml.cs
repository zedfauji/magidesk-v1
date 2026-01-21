using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Queries;

namespace Magidesk.Presentation.Controls;

public sealed partial class CategoryTreeView : UserControl
{
    public CategoryTreeViewModel ViewModel { get; }

    public CategoryTreeView()
    {
        this.InitializeComponent();
        ViewModel = (App.Services.GetService<CategoryTreeViewModel>() 
                    ?? throw new InvalidOperationException("CategoryTreeViewModel not registered in DI"));
        
        this.DataContext = ViewModel;
        this.Loaded += CategoryTreeView_Loaded;
    }

    private async void CategoryTreeView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Load categories when control is loaded
        await ViewModel.LoadCategoriesCommand.ExecuteAsync(null);
    }

    private async void TreeView_DragItemsCompleted(TreeView sender, TreeViewDragItemsCompletedEventArgs args)
    {
        // Handle drag-drop to change category parent
        if (args.Items.Count > 0 && args.Items[0] is CategoryNodeDto draggedCategory)
        {
            // Get the new parent (if dropped on a category) or null (if dropped at root)
            Guid? newParentId = args.NewParentItem is CategoryNodeDto parent ? parent.Id : null;
            
            // Call the ChangeParent command
            var parameters = new Tuple<Guid, Guid?>(draggedCategory.Id, newParentId);
            await ViewModel.ChangeParentCommand.ExecuteAsync(parameters);
        }
    }
}
