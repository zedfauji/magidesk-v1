using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;

namespace Magidesk.Infrastructure.Services;

public class PostgresBackupService : IBackupService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PostgresBackupService> _logger;
    private readonly string _backupDirectory;

    public PostgresBackupService(IConfiguration configuration, ILogger<PostgresBackupService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Configurable path, default to "Backups" in content root
        _backupDirectory = _configuration["BackupSettings:Directory"] ?? Path.Combine(AppContext.BaseDirectory, "Backups");
        
        if (!Directory.Exists(_backupDirectory))
        {
            Directory.CreateDirectory(_backupDirectory);
        }
    }

    public async Task<string> CreateBackupAsync(CancellationToken cancellationToken = default)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection string is missing.");
        }

        var dbInfo = ParseConnectionString(connectionString);
        
        var fileName = $"backup_{dbInfo.Database}_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
        var filePath = Path.Combine(_backupDirectory, fileName);

        // pg_dump command
        // We assume pg_dump is in PATH. If not, it needs to be configured.
        // For Windows POS, we might need full path if not in env.
        var pgDumpPath = _configuration["BackupSettings:PgDumpPath"] ?? "pg_dump";

        _logger.LogInformation("Starting database backup to {FilePath}", filePath);

        var startInfo = new ProcessStartInfo
        {
            FileName = pgDumpPath,
            // Format: -h host -p port -U username -F p -f "filepath" dbname
            // We ask for Plain text SQL (-F p) or Custom (-F c). Plain is more portable but larger. 
            // -F c (Custom) is better for restores. Let's use Custom.
            Arguments = $"-h {dbInfo.Host} -p {dbInfo.Port} -U {dbInfo.Username} -F c -f \"{filePath}\" {dbInfo.Database}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Set Password Env Var
        startInfo.EnvironmentVariables["PGPASSWORD"] = dbInfo.Password;

        using var process = new Process { StartInfo = startInfo };
        
        var errorOutput = new StringBuilder();
        process.ErrorDataReceived += (sender, args) => 
        {
            if (args.Data != null) errorOutput.AppendLine(args.Data);
        };

        process.Start();
        process.BeginErrorReadLine();
        
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            _logger.LogError("pg_dump failed: {Error}", errorOutput.ToString());
            throw new Exception($"Backup failed with exit code {process.ExitCode}. Error: {errorOutput}");
        }

        _logger.LogInformation("Database backup completed successfully.");
        return fileName;
    }

    public Task<List<BackupInfoDto>> ListBackupsAsync(CancellationToken cancellationToken = default)
    {
        var dirInfo = new DirectoryInfo(_backupDirectory);
        if (!dirInfo.Exists)
        {
            return Task.FromResult(new List<BackupInfoDto>());
        }

        var files = dirInfo.GetFiles("*.sql") // Or whatever extension based on format
            .OrderByDescending(f => f.CreationTime)
            .Select(f => new BackupInfoDto
            {
                FileName = f.Name,
                CreatedAt = f.CreationTime,
                SizeBytes = f.Length,
                Path = f.FullName
            })
            .ToList();

        return Task.FromResult(files);
    }

    public async Task RestoreBackupAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_backupDirectory, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Backup file not found: {fileName}");
        }
        
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("Connection string missing");
        
        var dbInfo = ParseConnectionString(connectionString);

        // pg_restore
        var pgRestorePath = _configuration["BackupSettings:PgRestorePath"] ?? "pg_restore";
        
        _logger.LogWarning("Starting database restore from {FilePath}", filePath);

        var startInfo = new ProcessStartInfo
        {
            FileName = pgRestorePath,
            // -c: Clean (drop) database objects before recreating
            // -d: Connect to database name
            Arguments = $"-h {dbInfo.Host} -p {dbInfo.Port} -U {dbInfo.Username} -c -d {dbInfo.Database} \"{filePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        startInfo.EnvironmentVariables["PGPASSWORD"] = dbInfo.Password;

        using var process = new Process { StartInfo = startInfo };
        var errorOutput = new StringBuilder();
        process.ErrorDataReceived += (s, a) => { if(a.Data != null) errorOutput.AppendLine(a.Data); };

        process.Start();
        process.BeginErrorReadLine();
        
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
             _logger.LogError("pg_restore failed: {Error}", errorOutput.ToString());
            throw new Exception($"Restore failed with exit code {process.ExitCode}. Error: {errorOutput}");
        }
        
        _logger.LogInformation("Database restore completed.");
    }

    private (string Host, string Port, string Database, string Username, string Password) ParseConnectionString(string connectionString)
    {
        var builder = new System.Data.Common.DbConnectionStringBuilder();
        builder.ConnectionString = connectionString;

        string Get(string[] keys, string @default = "")
        {
            foreach(var key in keys)
            {
                if (builder.ContainsKey(key)) return builder[key] as string ?? "";
            }
            return @default;
        }

        var host = Get(new[] { "Host", "Server", "Data Source" }, "localhost");
        var port = Get(new[] { "Port" }, "5432");
        var db = Get(new[] { "Database", "Initial Catalog" });
        var user = Get(new[] { "Username", "User Id", "User" });
        var pass = Get(new[] { "Password", "Password" });

        return (host, port, db, user, pass);
    }
}
