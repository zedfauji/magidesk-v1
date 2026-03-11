using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using Magidesk.Tests.E2E.Infrastructure.Models;
using Npgsql;

namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Captures comprehensive failure artifacts when tests fail.
/// </summary>
public sealed class FailureCaptureSystem
{
    private readonly string _artifactsDirectory;

    public FailureCaptureSystem(string artifactsDirectory)
    {
        _artifactsDirectory = artifactsDirectory ?? throw new ArgumentNullException(nameof(artifactsDirectory));
    }

    /// <summary>
    /// Captures all failure artifacts for a failed test.
    /// </summary>
    public void CaptureFailureArtifacts(
        string testName,
        Exception exception,
        Window? mainWindow,
        string? connectionString,
        Guid executionId = default)
    {
        try
        {
            var timestamp = DateTime.UtcNow;
            var directoryName = $"{SanitizeFileName(testName)}_{timestamp:yyyy-MM-ddTHH-mm-ss}";
            var artifactPath = Path.Combine(_artifactsDirectory, directoryName);

            Directory.CreateDirectory(artifactPath);

            CaptureFailureInfo(artifactPath, testName, exception, timestamp);
            CaptureScreenshot(artifactPath, mainWindow, connectionString, executionId);
            CaptureUITree(artifactPath, mainWindow, connectionString, executionId);
            CaptureProcessState(artifactPath, mainWindow, connectionString, executionId);
            CaptureDatabaseSnapshot(artifactPath, connectionString, executionId);

            Console.WriteLine($"Failure artifacts captured to: {artifactPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing failure artifacts: {ex.Message}");
        }
    }

    private void CaptureFailureInfo(string artifactPath, string testName, Exception exception, DateTime timestamp)
    {
        try
        {
            var machineInfo = new MachineInfo
            {
                OperatingSystem = RuntimeInformation.OSDescription,
                DotNetVersion = RuntimeInformation.FrameworkDescription,
                Architecture = RuntimeInformation.ProcessArchitecture.ToString()
            };

            var failureArtifact = new FailureArtifact
            {
                TestName = testName,
                Timestamp = timestamp,
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace ?? string.Empty,
                MachineInfo = machineInfo
            };

            var json = JsonSerializer.Serialize(failureArtifact, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(Path.Combine(artifactPath, "failure-info.json"), json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing failure info: {ex.Message}");
        }
    }

    private void CaptureScreenshot(string artifactPath, Window? mainWindow, string? connectionString, Guid executionId)
    {
        try
        {
            if (mainWindow == null)
            {
                Console.WriteLine("Cannot capture screenshot: main window is null");
                return;
            }

            var screenshot = mainWindow.Capture();
            var screenshotPath = Path.Combine(artifactPath, "screenshot.png");
            screenshot.Save(screenshotPath, System.Drawing.Imaging.ImageFormat.Png);

            // Insert artifact record into database
            if (executionId != Guid.Empty && !string.IsNullOrWhiteSpace(connectionString))
            {
                var fileInfo = new FileInfo(screenshotPath);
                InsertArtifactRecord(connectionString, executionId, "Screenshot", screenshotPath, fileInfo.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing screenshot: {ex.Message}");
        }
    }

    private void CaptureUITree(string artifactPath, Window? mainWindow, string? connectionString, Guid executionId)
    {
        try
        {
            if (mainWindow == null)
            {
                Console.WriteLine("Cannot capture UI tree: main window is null");
                return;
            }

            var uiTreePath = Path.Combine(artifactPath, "ui-tree.xml");
            
            // Use FlaUI's tree walker to capture UI structure
            using var writer = new StreamWriter(uiTreePath);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            writer.WriteLine("<UITree>");
            CaptureElementTree(writer, mainWindow, 0);
            writer.WriteLine("</UITree>");

            // Insert artifact record into database
            if (executionId != Guid.Empty && !string.IsNullOrWhiteSpace(connectionString))
            {
                var fileInfo = new FileInfo(uiTreePath);
                InsertArtifactRecord(connectionString, executionId, "UITree", uiTreePath, fileInfo.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing UI tree: {ex.Message}");
        }
    }

    private void CaptureElementTree(StreamWriter writer, AutomationElement element, int depth)
    {
        var indent = new string(' ', depth * 2);
        var automationId = element.Properties.AutomationId.ValueOrDefault ?? "null";
        var name = element.Properties.Name.ValueOrDefault ?? "null";
        var controlType = element.Properties.ControlType.ValueOrDefault;
        var controlTypeName = controlType != null ? controlType.ToString() : "null";
        
        writer.WriteLine($"{indent}<Element AutomationId=\"{automationId}\" Name=\"{name}\" ControlType=\"{controlTypeName}\" />");
        
        // Limit depth to prevent excessive file size
        if (depth < 10)
        {
            try
            {
                foreach (var child in element.FindAllChildren())
                {
                    CaptureElementTree(writer, child, depth + 1);
                }
            }
            catch
            {
                // Ignore errors walking children
            }
        }
    }

    private void CaptureProcessState(string artifactPath, Window? mainWindow, string? connectionString, Guid executionId)
    {
        try
        {
            if (mainWindow == null)
            {
                Console.WriteLine("Cannot capture process state: main window is null");
                return;
            }

            var processId = mainWindow.Properties.ProcessId.ValueOrDefault;
            if (processId == 0)
            {
                Console.WriteLine("Cannot capture process state: process ID is 0");
                return;
            }

            var process = Process.GetProcessById(processId);
            var processState = new ProcessState
            {
                ProcessId = processId,
                WorkingSetMemoryMB = process.WorkingSet64 / (1024 * 1024),
                CpuUsagePercent = 0, // CPU usage requires sampling over time
                ThreadCount = process.Threads.Count,
                TotalProcessorTime = process.TotalProcessorTime
            };

            var json = JsonSerializer.Serialize(processState, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var processStatePath = Path.Combine(artifactPath, "process-state.json");
            File.WriteAllText(processStatePath, json);

            // Insert artifact record into database
            if (executionId != Guid.Empty && !string.IsNullOrWhiteSpace(connectionString))
            {
                var fileInfo = new FileInfo(processStatePath);
                InsertArtifactRecord(connectionString, executionId, "ProcessState", processStatePath, fileInfo.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing process state: {ex.Message}");
        }
    }

    private void CaptureDatabaseSnapshot(string artifactPath, string? connectionString, Guid executionId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("Cannot capture database snapshot: connection string is null or empty");
                return;
            }

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var snapshotPath = Path.Combine(artifactPath, "database-snapshot.sql");
            using var writer = new StreamWriter(snapshotPath);

            // Capture key transactional tables
            var tables = new[] { "tickets", "order_lines", "payments", "cash_sessions" };
            foreach (var table in tables)
            {
                writer.WriteLine($"-- Table: {table}");
                using var cmd = new NpgsqlCommand($"SELECT * FROM {table}", connection);
                using var reader = cmd.ExecuteReader();

                var columnCount = reader.FieldCount;
                var columnNames = new string[columnCount];
                for (int i = 0; i < columnCount; i++)
                {
                    columnNames[i] = reader.GetName(i);
                }

                while (reader.Read())
                {
                    var values = new object[columnCount];
                    reader.GetValues(values);
                    writer.WriteLine($"-- {string.Join(", ", values)}");
                }

                writer.WriteLine();
            }

            writer.Close();

            // Insert artifact record into database
            if (executionId != Guid.Empty)
            {
                var fileInfo = new FileInfo(snapshotPath);
                InsertArtifactRecord(connectionString, executionId, "DatabaseSnapshot", snapshotPath, fileInfo.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing database snapshot: {ex.Message}");
        }
    }

    /// <summary>
    /// Inserts an artifact record into the test_artifacts table.
    /// </summary>
    private void InsertArtifactRecord(string connectionString, Guid executionId, string artifactType, string filePath, long fileSizeBytes)
    {
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            const string sql = @"
                INSERT INTO test_artifacts (
                    execution_id,
                    artifact_type,
                    file_path,
                    file_size_bytes
                )
                VALUES (
                    @executionId,
                    @artifactType,
                    @filePath,
                    @fileSizeBytes
                )";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@executionId", executionId);
            command.Parameters.AddWithValue("@artifactType", artifactType);
            command.Parameters.AddWithValue("@filePath", filePath);
            command.Parameters.AddWithValue("@fileSizeBytes", fileSizeBytes);

            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting artifact record for {artifactType}: {ex.Message}");
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }
}
