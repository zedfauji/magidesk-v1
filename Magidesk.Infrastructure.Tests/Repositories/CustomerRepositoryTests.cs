using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Repositories;

namespace Magidesk.Infrastructure.Tests.Repositories;

/// <summary>
/// Integration tests for CustomerRepository.
/// </summary>
[Collection("Database Tests")]
public class CustomerRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres;")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new CustomerRepository(_context);

        // Ensure a clean schema for each test run
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task AddAsync_ShouldCreateCustomer()
    {
        // Arrange
        var customer = Customer.Create("Jane", "Smith", "+15551234567", "jane@example.com");

        // Act
        await _repository.AddAsync(customer);

        // Assert
        var retrieved = await _repository.GetByIdAsync(customer.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(customer.Id, retrieved.Id);
        Assert.Equal(customer.Phone, retrieved.Phone);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingCustomers()
    {
        // Arrange
        var c1 = Customer.Create("Alice", "Wonder", "+10000000001");
        var c2 = Customer.Create("Bob", "Builder", "+10000000002", "bob@example.com");
        var c3 = Customer.Create("Charlie", "Brown", "+10000000003");
        
        await _repository.AddAsync(c1);
        await _repository.AddAsync(c2);
        await _repository.AddAsync(c3);

        // Act
        var results = await _repository.SearchAsync("Bob");

        // Assert
        Assert.Single(results);
        Assert.Equal(c2.Id, results.First().Id);
    }

    [Fact]
    public async Task GetByPhoneAsync_ShouldReturnCustomer()
    {
        // Arrange
        var phone = "+19998887777";
        var customer = Customer.Create("Long", "Island", phone);
        await _repository.AddAsync(customer);

        // Act
        var retrieved = await _repository.GetByPhoneAsync(phone);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(customer.Id, retrieved.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var customer = Customer.Create("New", "Customer", "+1112223333");
        await _repository.AddAsync(customer);

        // Act
        customer.UpdateContactInfo("new@email.com", customer.Phone, "Some Address");
        await _repository.UpdateAsync(customer);

        // Assert
        var retrieved = await _repository.GetByIdAsync(customer.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("new@email.com", retrieved.Email);
        Assert.Equal("Some Address", retrieved.Address);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
