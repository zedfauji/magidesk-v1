using System;
using System.Collections.Generic;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a historical record of a completed game session.
/// Used for analytics, reporting, and customer preference tracking.
/// </summary>
public class GameHistory
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid TableId { get; private set; }
    public GameType GameType { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public TimeSpan Duration { get; private set; }
    public int PlayerCount { get; private set; }
    public Money TotalCharge { get; private set; } = Money.Zero();
    public string? Winner { get; private set; }
    public Dictionary<string, object> GameData { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }

    // Private constructor for EF Core
    private GameHistory()
    {
    }

    /// <summary>
    /// Creates a new game history record.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <param name="tableId">ID of the table where the game was played</param>
    /// <param name="gameType">Type of game played</param>
    /// <param name="startTime">When the game started</param>
    /// <param name="playerCount">Number of players</param>
    /// <returns>New GameHistory instance</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    public static GameHistory Create(
        Guid sessionId,
        Guid tableId,
        GameType gameType,
        DateTime startTime,
        int playerCount)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(sessionId));
        }

        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        if (playerCount <= 0)
        {
            throw new ArgumentException("Player count must be greater than zero.", nameof(playerCount));
        }

        if (startTime > DateTime.UtcNow)
        {
            throw new ArgumentException("Start time cannot be in the future.", nameof(startTime));
        }

        return new GameHistory
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            TableId = tableId,
            GameType = gameType,
            StartTime = startTime,
            PlayerCount = playerCount,
            TotalCharge = Money.Zero(),
            GameData = new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Ends the game and records the final details.
    /// </summary>
    /// <param name="totalCharge">Total charge for the session</param>
    /// <param name="winner">Optional winner name</param>
    /// <exception cref="System.InvalidOperationException">Thrown when game is already ended</exception>
    /// <exception cref="ArgumentNullException">Thrown when totalCharge is null</exception>
    public void EndGame(Money totalCharge, string? winner = null)
    {
        if (totalCharge == null)
        {
            throw new ArgumentNullException(nameof(totalCharge));
        }

        if (EndTime != default)
        {
            throw new System.InvalidOperationException("Game has already been ended.");
        }

        EndTime = DateTime.UtcNow;
        Duration = EndTime - StartTime;
        TotalCharge = totalCharge;
        Winner = winner?.Trim();
    }

    /// <summary>
    /// Adds custom game data for analytics.
    /// </summary>
    /// <param name="key">Data key</param>
    /// <param name="value">Data value</param>
    /// <exception cref="ArgumentException">Thrown when key is empty</exception>
    public void AddGameData(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Game data key cannot be empty.", nameof(key));
        }

        GameData[key.Trim()] = value;
    }

    /// <summary>
    /// Gets game data by key.
    /// </summary>
    /// <param name="key">Data key</param>
    /// <returns>Data value or null if not found</returns>
    public object? GetGameData(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        return GameData.TryGetValue(key.Trim(), out var value) ? value : null;
    }

    /// <summary>
    /// Calculates the revenue per hour for this game.
    /// </summary>
    /// <returns>Revenue per hour</returns>
    public Money GetRevenuePerHour()
    {
        if (Duration.TotalHours <= 0)
        {
            return Money.Zero();
        }

        var hoursDecimal = (decimal)Duration.TotalHours;
        return TotalCharge / hoursDecimal;
    }

    /// <summary>
    /// Calculates the revenue per player for this game.
    /// </summary>
    /// <returns>Revenue per player</returns>
    public Money GetRevenuePerPlayer()
    {
        if (PlayerCount <= 0)
        {
            return Money.Zero();
        }

        return TotalCharge / PlayerCount;
    }
}