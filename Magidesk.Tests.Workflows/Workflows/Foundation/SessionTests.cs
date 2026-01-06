using System;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.ViewModels;
using Moq;
using Xunit;

namespace Magidesk.Tests.Workflows.Workflows.Foundation
{
    public class SessionTests : Infrastructure.WorkflowTestBase
    {
        [Fact]
        public async Task LoginSuccess_ShouldNavigateToDefaultView_WhenPinIsValid()
        {
            // GIVEN
            var validPin = "1234";
            var user = User.Create("testuser", "Test", "User", Guid.NewGuid());
            
            MockSecurityService.Setup(x => x.GetUserByPinAsync("ENC_" + validPin, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(user);
            
            MockTerminalContext.Setup(x => x.TerminalId).Returns(Guid.NewGuid());

            MockDefaultRoutingService.Setup(x => x.GetDefaultViewTypeAsync(It.IsAny<Guid?>()))
                .ReturnsAsync(typeof(Magidesk.Presentation.Views.OrderEntryPage));

            var vm = GetViewModel<LoginViewModel>();
            vm.Pin = validPin;

            // WHEN
            await ExtractTaskFromCommand(vm.LoginCommand);

            // THEN
            vm.ErrorMessage.Should().BeEmpty();
            MockUserService.VerifySet(x => x.CurrentUser = It.Is<UserDto>(u => u.Id == user.Id), Times.Once());
            MockNavigationService.Verify(x => x.Navigate(typeof(Magidesk.Presentation.Views.OrderEntryPage), It.IsAny<object>()), Times.Once());
        }

        [Fact]
        public async Task LoginFailure_ShouldShowError_WhenPinIsInvalid()
        {
            // GIVEN
            var invalidPin = "9999";
            MockSecurityService.Setup(x => x.GetUserByPinAsync("ENC_" + invalidPin, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync((User?)null);

            var vm = GetViewModel<LoginViewModel>();
            vm.Pin = invalidPin;

            // WHEN
            await ExtractTaskFromCommand(vm.LoginCommand);

            // THEN
            vm.ErrorMessage.Should().Be("Invalid PIN.");
            MockUserService.VerifySet(x => x.CurrentUser = It.IsAny<UserDto>(), Times.Never);
            MockNavigationService.Verify(x => x.Navigate(It.IsAny<Type>(), It.IsAny<object>()), Times.Never);
        }

        // Helper to execute AsyncRelayCommand which returns Task
        private async Task ExtractTaskFromCommand(System.Windows.Input.ICommand command)
        {
            if (command is CommunityToolkit.Mvvm.Input.IAsyncRelayCommand asyncCmd)
            {
                await asyncCmd.ExecuteAsync(null);
            }
            else
            {
                command.Execute(null);
            }
        }
    }
}
