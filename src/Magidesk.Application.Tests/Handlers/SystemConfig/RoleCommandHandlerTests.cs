using System;
using System.Threading.Tasks;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Handlers.SystemConfig;

public class RoleCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _mockRoleRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    // Mock UnitOfWork implicitly if repo handles it or add explicit mock

    public RoleCommandHandlerTests()
    {
        _mockRoleRepo = new Mock<IRoleRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
    }

    [Fact]
    public async Task CreateRole_ShouldSucceed_WhenNameIsValid()
    {
        // Arrange
        var handler = new CreateRoleCommandHandler(_mockRoleRepo.Object);
        var command = new CreateRoleCommand("Manager", UserPermission.VoidTicket);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.RoleId);
        _mockRoleRepo.Verify(r => r.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRole_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var handler = new CreateRoleCommandHandler(_mockRoleRepo.Object);
        var command = new CreateRoleCommand("", UserPermission.None);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        _mockRoleRepo.Verify(r => r.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRole_ShouldSucceed_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = Role.Create("Old Name", UserPermission.None);
        // Force ID via reflection or if Repo Mock returns it
        // Since Role.Id is private set, we might rely on the created instance effectively having a guid
        
        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>())).ReturnsAsync(role);
        
        var handler = new UpdateRoleCommandHandler(_mockRoleRepo.Object);
        var command = new UpdateRoleCommand(roleId, "New Name", UserPermission.CreateTicket);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", role.Name);
        Assert.Equal(UserPermission.CreateTicket, role.Permissions);
        _mockRoleRepo.Verify(r => r.UpdateAsync(role, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRole_ShouldSucceed_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = Role.Create("To Delete", UserPermission.None);
        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>())).ReturnsAsync(role);
        _mockUserRepo.Setup(u => u.HasUsersInRoleAsync(roleId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new DeleteRoleCommandHandler(_mockRoleRepo.Object, _mockUserRepo.Object);
        var command = new DeleteRoleCommand(roleId);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRoleRepo.Verify(r => r.DeleteAsync(role, It.IsAny<CancellationToken>()), Times.Once);
    }
}
