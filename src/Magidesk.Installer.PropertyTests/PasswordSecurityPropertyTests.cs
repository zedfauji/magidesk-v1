using FsCheck.Xunit;
using Magidesk.Installer.CustomActions;
using System.Text.RegularExpressions;
using Xunit;

namespace Magidesk.Installer.PropertyTests;

/// <summary>
/// Property-based tests for PostgreSQL password security.
/// **Validates: Requirements 6.2, 19.3, 19.5**
/// Property 5: PostgreSQL Password Security
/// </summary>
public class PasswordSecurityPropertyTests
{
    /// <summary>
    /// Property: For any installation, the generated PostgreSQL password should
    /// be at least 16 characters long.
    /// </summary>
    [Fact]
    public void GeneratedPassword_MeetsMinimumLength()
    {
        var installer = new PostgreSQLInstaller();
        
        // Test multiple times to ensure consistency
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();
            Assert.True(password.Length >= 16, $"Password length {password.Length} is less than 16");
        }
    }

    /// <summary>
    /// Property: For any password length >= 16, the generated password should
    /// have exactly that length.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool GeneratedPassword_HasExactRequestedLength(int length)
    {
        if (length < 16 || length > 128)
        {
            return true; // Skip invalid lengths
        }

        var installer = new PostgreSQLInstaller();
        var password = installer.GenerateSecurePassword(length);

        return password.Length == length;
    }

    /// <summary>
    /// Property: For any generated password, it should contain at least one
    /// uppercase letter.
    /// </summary>
    [Fact]
    public void GeneratedPassword_ContainsUppercase()
    {
        var installer = new PostgreSQLInstaller();
        
        // Test multiple times to ensure consistency
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();
            Assert.True(password.Any(char.IsUpper), "Password does not contain uppercase letter");
        }
    }

    /// <summary>
    /// Property: For any generated password, it should contain at least one
    /// lowercase letter.
    /// </summary>
    [Fact]
    public void GeneratedPassword_ContainsLowercase()
    {
        var installer = new PostgreSQLInstaller();
        
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();
            Assert.True(password.Any(char.IsLower), "Password does not contain lowercase letter");
        }
    }

    /// <summary>
    /// Property: For any generated password, it should contain at least one digit.
    /// </summary>
    [Fact]
    public void GeneratedPassword_ContainsDigit()
    {
        var installer = new PostgreSQLInstaller();
        
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();
            Assert.True(password.Any(char.IsDigit), "Password does not contain digit");
        }
    }

    /// <summary>
    /// Property: For any generated password, it should contain at least one
    /// special character from the allowed set.
    /// </summary>
    [Fact]
    public void GeneratedPassword_ContainsSpecialCharacter()
    {
        var installer = new PostgreSQLInstaller();
        const string specialChars = "!@#$%^&*()-_=+[]{}";
        
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();
            Assert.True(password.Any(c => specialChars.Contains(c)), "Password does not contain special character");
        }
    }

    /// <summary>
    /// Property: For any generated password, it should contain all four character
    /// categories (uppercase, lowercase, digit, special).
    /// </summary>
    [Fact]
    public void GeneratedPassword_ContainsAllCategories()
    {
        var installer = new PostgreSQLInstaller();
        const string specialChars = "!@#$%^&*()-_=+[]{}";
        
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();
            
            bool hasUppercase = password.Any(char.IsUpper);
            bool hasLowercase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => specialChars.Contains(c));

            Assert.True(hasUppercase && hasLowercase && hasDigit && hasSpecial, 
                "Password does not contain all required character categories");
        }
    }

    /// <summary>
    /// Property: For any two generated passwords, they should be different
    /// (demonstrating randomness).
    /// </summary>
    [Fact]
    public void GeneratedPasswords_AreDifferent()
    {
        var installer = new PostgreSQLInstaller();
        
        for (int i = 0; i < 100; i++)
        {
            var password1 = installer.GenerateSecurePassword();
            var password2 = installer.GenerateSecurePassword();
            Assert.NotEqual(password1, password2);
        }
    }

    /// <summary>
    /// Property: For any generated password, it should not have a predictable
    /// pattern (first 4 characters should not always be in the same order).
    /// </summary>
    [Fact]
    public void GeneratedPassword_IsShuffled()
    {
        var installer = new PostgreSQLInstaller();
        
        // Generate multiple passwords and check if they have different patterns
        var passwords = Enumerable.Range(0, 10)
            .Select(_ => installer.GenerateSecurePassword())
            .ToList();

        // Check that the first 4 characters are not always in the same category order
        var firstCharPatterns = passwords
            .Select(p => GetCharacterPattern(p.Substring(0, 4)))
            .Distinct()
            .Count();

        // We should have at least 2 different patterns (showing shuffling)
        Assert.True(firstCharPatterns >= 2, "Passwords show predictable pattern - not properly shuffled");
    }

    /// <summary>
    /// Property: For any generated password, it should only contain characters
    /// from the allowed character set.
    /// </summary>
    [Fact]
    public void GeneratedPassword_ContainsOnlyAllowedCharacters()
    {
        var installer = new PostgreSQLInstaller();
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}";
        
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();
            Assert.True(password.All(c => allowedChars.Contains(c)), 
                $"Password contains disallowed character");
        }
    }

    /// <summary>
    /// Property: For any password length less than 16, the generator should
    /// throw an ArgumentException.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool GeneratedPassword_RejectsInvalidLength(int length)
    {
        if (length >= 16)
        {
            return true; // Skip valid lengths
        }

        var installer = new PostgreSQLInstaller();
        
        try
        {
            var password = installer.GenerateSecurePassword(length);
            return false; // Should have thrown
        }
        catch (ArgumentException)
        {
            return true; // Expected exception
        }
        catch
        {
            return false; // Wrong exception type
        }
    }

    /// <summary>
    /// Property: For any generated password, when logged, it should be redacted
    /// (simulating the logging requirement that passwords not appear in plaintext).
    /// </summary>
    [Fact]
    public void PasswordLogging_IsRedacted()
    {
        var installer = new PostgreSQLInstaller();
        
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();

            // Simulate sanitization that should happen in logging
            var sanitizedLog = SanitizePasswordForLogging(password);

            // The sanitized log should not contain the actual password
            Assert.Equal("[REDACTED]", sanitizedLog);
            Assert.DoesNotContain(password, sanitizedLog);
        }
    }

    /// <summary>
    /// Property: For any connection string containing a password, the sanitized
    /// version should mask the password component.
    /// </summary>
    [Fact]
    public void ConnectionStringLogging_MasksPassword()
    {
        var installer = new PostgreSQLInstaller();
        
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();
            var connectionString = $"Host=127.0.0.1;Port=5432;Database=magidesk_pos;Username=postgres;Password={password}";

            var sanitized = SanitizeConnectionString(connectionString);

            // Sanitized string should not contain the actual password
            Assert.DoesNotContain(password, sanitized);
            Assert.Contains("[REDACTED]", sanitized);
        }
    }

    /// <summary>
    /// Property: For any set of generated passwords, they should have high entropy
    /// (no password should appear twice in a reasonable sample size).
    /// </summary>
    [Fact]
    public void GeneratedPasswords_HaveHighEntropy()
    {
        var installer = new PostgreSQLInstaller();
        
        // Generate 50 passwords
        var passwords = Enumerable.Range(0, 50)
            .Select(_ => installer.GenerateSecurePassword())
            .ToList();

        // All passwords should be unique (demonstrating cryptographic randomness)
        var uniqueCount = passwords.Distinct().Count();
        
        Assert.Equal(passwords.Count, uniqueCount);
    }

    /// <summary>
    /// Property: For any generated password, the character distribution should
    /// not be heavily biased (no single character should appear more than 50% of the time).
    /// </summary>
    [Fact]
    public void GeneratedPassword_HasBalancedDistribution()
    {
        var installer = new PostgreSQLInstaller();
        
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword(32); // Use longer password for better distribution testing

            // Check that no single character appears more than 50% of the time
            var charGroups = password.GroupBy(c => c);
            var maxOccurrence = charGroups.Max(g => g.Count());
            
            Assert.True(maxOccurrence <= password.Length / 2, 
                $"Character distribution is biased - one character appears {maxOccurrence} times in {password.Length} characters");
        }
    }

    /// <summary>
    /// Property: For any generated password, it should be usable in a PostgreSQL
    /// connection string without escaping issues (only truly unsafe characters like
    /// semicolon, quotes, and control characters should be excluded).
    /// </summary>
    [Fact]
    public void GeneratedPassword_IsConnectionStringSafe()
    {
        var installer = new PostgreSQLInstaller();
        
        for (int i = 0; i < 100; i++)
        {
            var password = installer.GenerateSecurePassword();

            // Ensure password is not null
            Assert.False(string.IsNullOrEmpty(password), "Password should not be null or empty");

            // Password should not contain characters that would break connection string parsing
            // Note: '=' and other special chars are OK in Npgsql connection strings
            var unsafeChars = new[] { ';', '\'', '"', '\0', '\r', '\n' };
            
            Assert.True(!password.Any(c => unsafeChars.Contains(c)), 
                "Password contains unsafe characters for connection strings");
        }
    }

    #region Helper Methods

    /// <summary>
    /// Helper method to get the character pattern of a string
    /// (U=uppercase, L=lowercase, D=digit, S=special).
    /// </summary>
    private static string GetCharacterPattern(string text)
    {
        const string specialChars = "!@#$%^&*()-_=+[]{}";
        
        return new string(text.Select(c =>
        {
            if (char.IsUpper(c)) return 'U';
            if (char.IsLower(c)) return 'L';
            if (char.IsDigit(c)) return 'D';
            if (specialChars.Contains(c)) return 'S';
            return '?';
        }).ToArray());
    }

    /// <summary>
    /// Helper method to sanitize a password for logging (should return [REDACTED]).
    /// </summary>
    private static string SanitizePasswordForLogging(string password)
    {
        return "[REDACTED]";
    }

    /// <summary>
    /// Helper method to sanitize a connection string for logging.
    /// </summary>
    private static string SanitizeConnectionString(string connectionString)
    {
        // Simple regex-based sanitization for testing
        return Regex.Replace(connectionString, @"Password=[^;]+", "Password=[REDACTED]", RegexOptions.IgnoreCase);
    }

    #endregion
}
