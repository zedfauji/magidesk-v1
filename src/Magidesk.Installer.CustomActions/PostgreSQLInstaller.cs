using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using WixToolset.Dtf.WindowsInstaller;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Custom action for PostgreSQL 16 installation and configuration.
/// Extracts binaries, initializes database, configures security, and registers Windows service.
/// </summary>
public class PostgreSQLInstaller : IPostgreSQLInstaller
{
    private const string DefaultServiceName = "magidesk_postgres";
    private const string DefaultServiceAccount = @"NT AUTHORITY\NetworkService";
    private const int DefaultPort = 5432;
    private const string DefaultListenAddress = "127.0.0.1";

    /// <summary>
    /// Installs PostgreSQL 16 to the specified directory.
    /// Extracts binaries, initializes data directory, configures security, and registers Windows service.
    /// </summary>
    /// <param name="installPath">Target installation directory (e.g., C:\Program Files\PostgreSQL\16)</param>
    /// <param name="dataPath">PostgreSQL data directory (e.g., C:\ProgramData\Magidesk\PostgreSQL\data)</param>
    /// <returns>Installation result with generated password and service name</returns>
    public async Task<PostgreSQLInstallResult> InstallAsync(string installPath, string dataPath)
    {
        try
        {
            // Step 1: Extract PostgreSQL binaries
            await ExtractPostgreSQLBinariesAsync(installPath);

            // Step 2: Generate secure password
            string password = GenerateSecurePassword();

            // Step 3: Initialize data directory
            await InitializeDataDirectoryAsync(installPath, dataPath, password);

            // Step 4: Configure postgresql.conf for localhost-only access
            ConfigurePostgreSQLConf(dataPath);

            // Step 5: Configure pg_hba.conf for secure authentication
            ConfigurePgHbaConf(dataPath);

            // Step 6: Register Windows service
            var serviceResult = await RegisterServiceAsync(
                DefaultServiceName,
                Path.Combine(installPath, "bin", "pg_ctl.exe"),
                dataPath);

            if (!serviceResult.Success)
            {
                return new PostgreSQLInstallResult(
                    false,
                    password,
                    DefaultServiceName,
                    serviceResult.ErrorMessage);
            }

            // Step 7: Start service and verify
            var startResult = await StartAndVerifyServiceAsync(DefaultServiceName);
            if (!startResult.Success)
            {
                return new PostgreSQLInstallResult(
                    false,
                    password,
                    DefaultServiceName,
                    startResult.ErrorMessage);
            }

            return new PostgreSQLInstallResult(
                true,
                password,
                DefaultServiceName,
                null);
        }
        catch (Exception ex)
        {
            return new PostgreSQLInstallResult(
                false,
                string.Empty,
                DefaultServiceName,
                $"PostgreSQL installation failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates a cryptographically secure password.
    /// </summary>
    /// <param name="length">Password length (minimum 16)</param>
    /// <returns>Generated password with uppercase, lowercase, digits, and special characters</returns>
    public string GenerateSecurePassword(int length = 16)
    {
        if (length < 16)
        {
            throw new ArgumentException("Password length must be at least 16 characters", nameof(length));
        }

        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*()-_=+[]{}";
        const string allChars = uppercase + lowercase + digits + special;

        using var rng = RandomNumberGenerator.Create();
        var password = new char[length];

        // Ensure at least one character from each category
        password[0] = uppercase[GetRandomIndex(rng, uppercase.Length)];
        password[1] = lowercase[GetRandomIndex(rng, lowercase.Length)];
        password[2] = digits[GetRandomIndex(rng, digits.Length)];
        password[3] = special[GetRandomIndex(rng, special.Length)];

        // Fill remaining with random characters
        for (int i = 4; i < length; i++)
        {
            password[i] = allChars[GetRandomIndex(rng, allChars.Length)];
        }

        // Shuffle to avoid predictable pattern
        return new string(password.OrderBy(_ => GetRandomIndex(rng, length)).ToArray());
    }

    /// <summary>
    /// Registers PostgreSQL as a Windows service.
    /// </summary>
    /// <param name="serviceName">Service name (e.g., magidesk_postgres)</param>
    /// <param name="binPath">Path to pg_ctl.exe</param>
    /// <param name="dataPath">PostgreSQL data directory</param>
    /// <returns>Service registration result</returns>
    public async Task<ServiceRegistrationResult> RegisterServiceAsync(
        string serviceName,
        string binPath,
        string dataPath)
    {
        try
        {
            // Use pg_ctl to register the service
            var startInfo = new ProcessStartInfo
            {
                FileName = binPath,
                Arguments = $"register -N \"{serviceName}\" -D \"{dataPath}\" -U \"{DefaultServiceAccount}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return new ServiceRegistrationResult(
                    false,
                    serviceName,
                    "Failed to start pg_ctl process for service registration");
            }

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                return new ServiceRegistrationResult(
                    false,
                    serviceName,
                    $"Service registration failed with exit code {process.ExitCode}: {error}");
            }

            // Configure service to start automatically
            await ConfigureServiceStartupAsync(serviceName);

            return new ServiceRegistrationResult(true, serviceName, null);
        }
        catch (Exception ex)
        {
            return new ServiceRegistrationResult(
                false,
                serviceName,
                $"Exception during service registration: {ex.Message}");
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Extracts PostgreSQL ZIP to the installation directory.
    /// </summary>
    private async Task ExtractPostgreSQLBinariesAsync(string installPath)
    {
        // This will be called from the WiX custom action with the ZIP path
        // For now, we assume the ZIP is extracted by WiX or provided separately
        // The actual extraction logic would go here if needed
        await Task.CompletedTask;
    }

    /// <summary>
    /// Initializes PostgreSQL data directory using initdb.
    /// </summary>
    private async Task InitializeDataDirectoryAsync(string installPath, string dataPath, string password)
    {
        // Create data directory if it doesn't exist
        Directory.CreateDirectory(dataPath);

        // Write password to temporary file for initdb
        string tempPasswordFile = Path.Combine(Path.GetTempPath(), $"pg_pwd_{Guid.NewGuid()}.txt");
        try
        {
            await File.WriteAllTextAsync(tempPasswordFile, password);

            // Run initdb to initialize the data directory
            string initdbPath = Path.Combine(installPath, "bin", "initdb.exe");
            var startInfo = new ProcessStartInfo
            {
                FileName = initdbPath,
                Arguments = $"-D \"{dataPath}\" -U postgres -A scram-sha-256 --pwfile=\"{tempPasswordFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start initdb process");
            }

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"initdb failed with exit code {process.ExitCode}: {error}");
            }
        }
        finally
        {
            // Delete temporary password file
            if (File.Exists(tempPasswordFile))
            {
                File.Delete(tempPasswordFile);
            }
        }
    }

    /// <summary>
    /// Configures postgresql.conf for localhost-only access.
    /// </summary>
    private void ConfigurePostgreSQLConf(string dataPath)
    {
        string configPath = Path.Combine(dataPath, "postgresql.conf");
        
        var configLines = new List<string>
        {
            $"listen_addresses = '{DefaultListenAddress}'",
            $"port = {DefaultPort}",
            "max_connections = 100",
            "shared_buffers = 128MB",
            "log_destination = 'stderr'",
            "logging_collector = on",
            "log_directory = 'log'",
            "log_filename = 'postgresql-%Y-%m-%d.log'",
            "log_rotation_age = 1d",
            "log_rotation_size = 10MB"
        };

        // Append configuration to postgresql.conf
        File.AppendAllLines(configPath, new[] { "", "# Magidesk POS Configuration", "" });
        File.AppendAllLines(configPath, configLines);
    }

    /// <summary>
    /// Configures pg_hba.conf for secure authentication.
    /// </summary>
    private void ConfigurePgHbaConf(string dataPath)
    {
        string hbaPath = Path.Combine(dataPath, "pg_hba.conf");
        
        var hbaLines = new List<string>
        {
            "",
            "# Magidesk POS Configuration - Localhost only with SCRAM-SHA-256",
            "# TYPE  DATABASE        USER            ADDRESS                 METHOD",
            "local   all             all                                     scram-sha-256",
            "host    all             all             127.0.0.1/32            scram-sha-256",
            "host    all             all             ::1/128                 scram-sha-256"
        };

        // Replace the default pg_hba.conf with our secure configuration
        File.WriteAllLines(hbaPath, hbaLines);
    }

    /// <summary>
    /// Configures service to start automatically.
    /// </summary>
    private async Task ConfigureServiceStartupAsync(string serviceName)
    {
        // Use sc.exe to configure automatic startup
        var startInfo = new ProcessStartInfo
        {
            FileName = "sc.exe",
            Arguments = $"config {serviceName} start= auto",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process != null)
        {
            await process.WaitForExitAsync();
        }
    }

    /// <summary>
    /// Starts the PostgreSQL service and verifies it's running.
    /// Retries up to 3 times with 5-second delays.
    /// </summary>
    private async Task<ServiceRegistrationResult> StartAndVerifyServiceAsync(string serviceName)
    {
        const int maxRetries = 3;
        const int retryDelaySeconds = 5;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var sc = new ServiceController(serviceName);
                
                // Start the service if it's not running
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                }

                // Verify service is running
                sc.Refresh();
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    return new ServiceRegistrationResult(true, serviceName, null);
                }
            }
            catch (Exception ex)
            {
                if (attempt == maxRetries)
                {
                    return new ServiceRegistrationResult(
                        false,
                        serviceName,
                        $"Service failed to start after {maxRetries} attempts: {ex.Message}");
                }

                // Wait before retrying
                await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
            }
        }

        return new ServiceRegistrationResult(
            false,
            serviceName,
            $"Service failed to start after {maxRetries} attempts");
    }

    /// <summary>
    /// Gets a cryptographically random index for character selection.
    /// </summary>
    private int GetRandomIndex(RandomNumberGenerator rng, int maxValue)
    {
        var data = new byte[4];
        rng.GetBytes(data);
        return (int)(BitConverter.ToUInt32(data, 0) % maxValue);
    }

    #endregion
}

/// <summary>
/// Interface for PostgreSQL installation operations.
/// </summary>
public interface IPostgreSQLInstaller
{
    Task<PostgreSQLInstallResult> InstallAsync(string installPath, string dataPath);
    string GenerateSecurePassword(int length = 16);
    Task<ServiceRegistrationResult> RegisterServiceAsync(string serviceName, string binPath, string dataPath);
}

/// <summary>
/// Result of PostgreSQL installation operation.
/// </summary>
public record PostgreSQLInstallResult(
    bool Success,
    string GeneratedPassword,
    string ServiceName,
    string? ErrorMessage = null);

/// <summary>
/// Result of Windows service registration operation.
/// </summary>
public record ServiceRegistrationResult(
    bool Success,
    string ServiceName,
    string? ErrorMessage = null);
