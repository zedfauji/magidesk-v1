namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Defines the contract for database reset operations in test scenarios.
/// Restores database to clean baseline state before each test.
/// </summary>
public interface IDatabaseResetEngine
{
    /// <summary>
    /// Resets the database to clean state by deleting transactional data and seeding baseline configuration.
    /// Executes within a transaction to ensure atomicity.
    /// </summary>
    /// <exception cref="DatabaseResetException">Thrown when database reset fails.</exception>
    void ResetDatabase();
}
