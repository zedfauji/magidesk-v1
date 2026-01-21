using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Repositories;

namespace Magidesk.Infrastructure.Tests.Repositories;

/// <summary>
/// Integration tests for GameHistoryRepository.
/// </summary>
[Collection("Database Tests")]
public class GameHistoryRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GameHistoryRepository _repository;

    public GameHistoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new GameHistoryRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldCreateGameHistory()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddHours(-2);
        
        var gameHistory = GameHistory.Create(sessionId, tableId, GameType.EightBall, startTime, 2);
        gameHistory.EndGame(new Money(25.50m), "Player 1");

        // Act
        await _repository.AddAsync(gameHistory);

        // Assert
        var retrieved = await _repository.GetByIdAsync(gameHistory.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(gameHistory.Id, retrieved.Id);
        Assert.Equal(sessionId, retrieved.SessionId);
        Assert.Equal(tableId, retrieved.TableId);
        Assert.Equal(GameType.EightBall, retrieved.GameType);
        Assert.Equal(2, retrieved.PlayerCount);
        Assert.Equal("Player 1", retrieved.Winner);
    }

    [Fact]
    public async Task GetGameHistoryBySessionIdAsync_ShouldReturnCorrectHistory()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var gameHistory = GameHistory.Create(sessionId, tableId, GameType.NineBall, DateTime.UtcNow.AddHours(-1), 4);
        gameHistory.EndGame(new Money(40.00m));

        await _repository.AddAsync(gameHistory);

        // Act
        var retrieved = await _repository.GetGameHistoryBySessionIdAsync(sessionId);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(gameHistory.Id, retrieved.Id);
        Assert.Equal(sessionId, retrieved.SessionId);
    }

    [Fact]
    public async Task GetGameHistoryByTableIdAsync_ShouldReturnHistoryInDateRange()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var fromDate = DateTime.UtcNow.AddDays(-7);
        var toDate = DateTime.UtcNow;

        var history1 = GameHistory.Create(Guid.NewGuid(), tableId, GameType.EightBall, fromDate.AddDays(1), 2);
        var history2 = GameHistory.Create(Guid.NewGuid(), tableId, GameType.NineBall, fromDate.AddDays(3), 3);
        var history3 = GameHistory.Create(Guid.NewGuid(), tableId, GameType.Straight, fromDate.AddDays(-10), 2); // Outside range

        history1.EndGame(new Money(20.00m));
        history2.EndGame(new Money(30.00m));
        history3.EndGame(new Money(15.00m));

        await _repository.AddAsync(history1);
        await _repository.AddAsync(history2);
        await _repository.AddAsync(history3);

        // Act
        var results = await _repository.GetGameHistoryByTableIdAsync(tableId, fromDate, toDate);

        // Assert
        Assert.Equal(2, results.Count());
        Assert.Contains(results, h => h.Id == history1.Id);
        Assert.Contains(results, h => h.Id == history2.Id);
        Assert.DoesNotContain(results, h => h.Id == history3.Id);
    }

    [Fact]
    public async Task GetGameHistoryByTypeAsync_ShouldReturnCorrectGameType()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(-7);
        var toDate = DateTime.UtcNow;

        var eightBallHistory1 = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.EightBall, fromDate.AddDays(1), 2);
        var eightBallHistory2 = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.EightBall, fromDate.AddDays(2), 2);
        var nineBallHistory = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.NineBall, fromDate.AddDays(3), 3);

        eightBallHistory1.EndGame(new Money(20.00m));
        eightBallHistory2.EndGame(new Money(25.00m));
        nineBallHistory.EndGame(new Money(30.00m));

        await _repository.AddAsync(eightBallHistory1);
        await _repository.AddAsync(eightBallHistory2);
        await _repository.AddAsync(nineBallHistory);

        // Act
        var eightBallResults = await _repository.GetGameHistoryByTypeAsync(GameType.EightBall, fromDate, toDate);

        // Assert
        Assert.Equal(2, eightBallResults.Count());
        Assert.All(eightBallResults, h => Assert.Equal(GameType.EightBall, h.GameType));
    }

    [Fact]
    public async Task GetPopularGameTypesAsync_ShouldReturnCorrectData()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(-7);
        var toDate = DateTime.UtcNow;

        // Create multiple games of different types
        var eightBall1 = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.EightBall, fromDate.AddDays(1), 2);
        var eightBall2 = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.EightBall, fromDate.AddDays(2), 3);
        var eightBall3 = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.EightBall, fromDate.AddDays(3), 2);
        var nineBall1 = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.NineBall, fromDate.AddDays(4), 4);
        var nineBall2 = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.NineBall, fromDate.AddDays(5), 2);

        eightBall1.EndGame(new Money(20.00m));
        eightBall2.EndGame(new Money(30.00m));
        eightBall3.EndGame(new Money(25.00m));
        nineBall1.EndGame(new Money(40.00m));
        nineBall2.EndGame(new Money(20.00m));

        await _repository.AddAsync(eightBall1);
        await _repository.AddAsync(eightBall2);
        await _repository.AddAsync(eightBall3);
        await _repository.AddAsync(nineBall1);
        await _repository.AddAsync(nineBall2);

        // Act
        var popularTypes = await _repository.GetPopularGameTypesAsync(fromDate, toDate, 10);

        // Assert
        Assert.Equal(2, popularTypes.Count());
        
        var eightBallData = popularTypes.First(p => p.GameType == GameType.EightBall);
        Assert.Equal(3, eightBallData.SessionCount);
        Assert.Equal(75.00m, eightBallData.TotalRevenue.Amount);
        
        var nineBallData = popularTypes.First(p => p.GameType == GameType.NineBall);
        Assert.Equal(2, nineBallData.SessionCount);
        Assert.Equal(60.00m, nineBallData.TotalRevenue.Amount);
    }

    [Fact]
    public async Task GetAverageSessionDurationByGameTypeAsync_ShouldCalculateCorrectAverages()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(-7);
        var toDate = DateTime.UtcNow;
        var baseTime = DateTime.UtcNow.AddDays(-5);

        var eightBall1 = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.EightBall, baseTime, 2);
        var eightBall2 = GameHistory.Create(Guid.NewGuid(), Guid.NewGuid(), GameType.EightBall, baseTime.AddHours(1), 2);

        // End games with different durations
        eightBall1.EndGame(new Money(20.00m)); // Will have duration from creation to EndGame call
        await Task.Delay(10); // Small delay to ensure different end times
        eightBall2.EndGame(new Money(25.00m));

        await _repository.AddAsync(eightBall1);
        await _repository.AddAsync(eightBall2);

        // Act
        var durationData = await _repository.GetAverageSessionDurationByGameTypeAsync(fromDate, toDate);

        // Assert
        var eightBallData = durationData.FirstOrDefault(d => d.GameType == GameType.EightBall);
        Assert.NotNull(eightBallData);
        Assert.Equal(2, eightBallData.SessionCount);
        Assert.True(eightBallData.AverageDuration > TimeSpan.Zero);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}