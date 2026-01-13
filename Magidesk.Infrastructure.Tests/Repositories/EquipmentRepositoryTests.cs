using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Repositories;

namespace Magidesk.Infrastructure.Tests.Repositories;

/// <summary>
/// Integration tests for EquipmentRepository.
/// </summary>
[Collection("Database Tests")]
public class EquipmentRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EquipmentRepository _repository;

    public EquipmentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EquipmentRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldCreateEquipment()
    {
        // Arrange
        var equipment = Equipment.Create("Pool Cue Set", EquipmentType.Cue, "Professional pool cue set");

        // Act
        await _repository.AddAsync(equipment);

        // Assert
        var retrieved = await _repository.GetByIdAsync(equipment.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(equipment.Id, retrieved.Id);
        Assert.Equal(equipment.Name, retrieved.Name);
        Assert.Equal(EquipmentType.Cue, retrieved.Type);
        Assert.Equal(EquipmentStatus.Available, retrieved.Status);
    }

    [Fact]
    public async Task GetEquipmentByTypeAsync_ShouldReturnCorrectEquipment()
    {
        // Arrange
        var cue1 = Equipment.Create("Cue 1", EquipmentType.Cue);
        var cue2 = Equipment.Create("Cue 2", EquipmentType.Cue);
        var ballSet = Equipment.Create("Ball Set", EquipmentType.BallSet);

        await _repository.AddAsync(cue1);
        await _repository.AddAsync(cue2);
        await _repository.AddAsync(ballSet);

        // Act
        var cues = await _repository.GetEquipmentByTypeAsync(EquipmentType.Cue);

        // Assert
        Assert.Equal(2, cues.Count());
        Assert.All(cues, e => Assert.Equal(EquipmentType.Cue, e.Type));
    }

    [Fact]
    public async Task GetEquipmentByStatusAsync_ShouldReturnCorrectEquipment()
    {
        // Arrange
        var equipment1 = Equipment.Create("Equipment 1", EquipmentType.Cue);
        var equipment2 = Equipment.Create("Equipment 2", EquipmentType.BallSet);
        
        await _repository.AddAsync(equipment1);
        await _repository.AddAsync(equipment2);

        // Assign one equipment to simulate in-use status
        var tableId = Guid.NewGuid();
        equipment1.AssignToTable(tableId);
        await _repository.UpdateAsync(equipment1);

        // Act
        var availableEquipment = await _repository.GetEquipmentByStatusAsync(EquipmentStatus.Available);
        var inUseEquipment = await _repository.GetEquipmentByStatusAsync(EquipmentStatus.InUse);

        // Assert
        Assert.Single(availableEquipment);
        Assert.Equal(equipment2.Id, availableEquipment.First().Id);
        
        Assert.Single(inUseEquipment);
        Assert.Equal(equipment1.Id, inUseEquipment.First().Id);
    }

    [Fact]
    public async Task GetEquipmentByTableIdAsync_ShouldReturnAssignedEquipment()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var equipment1 = Equipment.Create("Equipment 1", EquipmentType.Cue);
        var equipment2 = Equipment.Create("Equipment 2", EquipmentType.BallSet);
        var equipment3 = Equipment.Create("Equipment 3", EquipmentType.Rack);

        await _repository.AddAsync(equipment1);
        await _repository.AddAsync(equipment2);
        await _repository.AddAsync(equipment3);

        // Assign equipment to table
        equipment1.AssignToTable(tableId);
        equipment2.AssignToTable(tableId);
        await _repository.UpdateAsync(equipment1);
        await _repository.UpdateAsync(equipment2);

        // Act
        var tableEquipment = await _repository.GetEquipmentByTableIdAsync(tableId);

        // Assert
        Assert.Equal(2, tableEquipment.Count());
        Assert.Contains(tableEquipment, e => e.Id == equipment1.Id);
        Assert.Contains(tableEquipment, e => e.Id == equipment2.Id);
    }

    [Fact]
    public async Task GetAvailableEquipmentByTypeAsync_ShouldReturnOnlyAvailableEquipment()
    {
        // Arrange
        var cue1 = Equipment.Create("Available Cue", EquipmentType.Cue);
        var cue2 = Equipment.Create("In Use Cue", EquipmentType.Cue);
        var cue3 = Equipment.Create("Maintenance Cue", EquipmentType.Cue);

        await _repository.AddAsync(cue1);
        await _repository.AddAsync(cue2);
        await _repository.AddAsync(cue3);

        // Set different statuses
        cue2.AssignToTable(Guid.NewGuid());
        cue3.ScheduleMaintenance(DateTime.UtcNow.AddDays(1));
        
        await _repository.UpdateAsync(cue2);
        await _repository.UpdateAsync(cue3);

        // Act
        var availableCues = await _repository.GetAvailableEquipmentByTypeAsync(EquipmentType.Cue);

        // Assert
        Assert.Single(availableCues);
        Assert.Equal(cue1.Id, availableCues.First().Id);
    }

    [Fact]
    public async Task GetEquipmentRequiringMaintenanceAsync_ShouldReturnEquipmentDueSoon()
    {
        // Arrange
        var equipment1 = Equipment.Create("Equipment 1", EquipmentType.Cue);
        var equipment2 = Equipment.Create("Equipment 2", EquipmentType.BallSet);
        var equipment3 = Equipment.Create("Equipment 3", EquipmentType.Rack);

        await _repository.AddAsync(equipment1);
        await _repository.AddAsync(equipment2);
        await _repository.AddAsync(equipment3);

        // Schedule maintenance
        equipment1.ScheduleMaintenance(DateTime.UtcNow.AddDays(3)); // Within 7 days
        equipment2.ScheduleMaintenance(DateTime.UtcNow.AddDays(10)); // Beyond 7 days
        // equipment3 has no scheduled maintenance

        await _repository.UpdateAsync(equipment1);
        await _repository.UpdateAsync(equipment2);

        // Act
        var maintenanceRequired = await _repository.GetEquipmentRequiringMaintenanceAsync(7);

        // Assert
        Assert.Single(maintenanceRequired);
        Assert.Equal(equipment1.Id, maintenanceRequired.First().Id);
    }

    [Fact]
    public async Task IsEquipmentAvailableAsync_ShouldReturnCorrectStatus()
    {
        // Arrange
        var availableEquipment = Equipment.Create("Available", EquipmentType.Cue);
        var inUseEquipment = Equipment.Create("In Use", EquipmentType.Cue);
        var inactiveEquipment = Equipment.Create("Inactive", EquipmentType.Cue);

        await _repository.AddAsync(availableEquipment);
        await _repository.AddAsync(inUseEquipment);
        await _repository.AddAsync(inactiveEquipment);

        inUseEquipment.AssignToTable(Guid.NewGuid());
        inactiveEquipment.Deactivate();

        await _repository.UpdateAsync(inUseEquipment);
        await _repository.UpdateAsync(inactiveEquipment);

        // Act & Assert
        Assert.True(await _repository.IsEquipmentAvailableAsync(availableEquipment.Id));
        Assert.False(await _repository.IsEquipmentAvailableAsync(inUseEquipment.Id));
        Assert.False(await _repository.IsEquipmentAvailableAsync(inactiveEquipment.Id));
        Assert.False(await _repository.IsEquipmentAvailableAsync(Guid.NewGuid())); // Non-existent
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}