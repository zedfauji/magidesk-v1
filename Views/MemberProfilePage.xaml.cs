using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;
using System;

namespace Magidesk.Presentation.Views;

public sealed partial class MemberProfilePage : Page
{
    public MemberProfileViewModel ViewModel { get; }

    public MemberProfilePage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MemberProfileViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Guid customerId)
        {
            if (ViewModel.LoadMemberCommand.CanExecute(customerId))
            {
                ViewModel.LoadMemberCommand.Execute(customerId);
            }
        }
    }
}
