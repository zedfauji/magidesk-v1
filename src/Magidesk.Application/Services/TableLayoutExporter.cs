using System.Text.Json;
using System.Text.Json.Serialization;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Services;

public class TableLayoutExporter
{
    private readonly JsonSerializerOptions _jsonOptions;

    public TableLayoutExporter()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// Exports a table layout to JSON format
    /// </summary>
    public string ExportToJson(TableLayoutDto layout)
    {
        if (layout == null)
        {
            throw new ArgumentNullException(nameof(layout));
        }

        var exportData = new TableLayoutExportData
        {
            Metadata = new LayoutMetadata
            {
                Name = layout.Name,
                Description = $"Exported on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                Version = layout.Version,
                ExportDate = DateTime.UtcNow,
                TableCount = layout.Tables.Count,
                FloorId = layout.FloorId,
                Software = "Magidesk POS",
                SoftwareVersion = "1.0.0"
            },
            Layout = layout
        };

        return JsonSerializer.Serialize(exportData, _jsonOptions);
    }

    /// <summary>
    /// Imports a table layout from JSON format
    /// </summary>
    public TableLayoutDto ImportFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON data cannot be null or empty.", nameof(json));
        }

        try
        {
            var exportData = JsonSerializer.Deserialize<TableLayoutExportData>(json, _jsonOptions);
            
            if (exportData?.Layout == null)
            {
                throw new InvalidOperationException("Invalid layout data in JSON file.");
            }

            // Validate the imported layout
            ValidateImportedLayout(exportData.Layout);

            return exportData.Layout;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid JSON format.", ex);
        }
    }

    /// <summary>
    /// Exports multiple layouts to a single JSON file
    /// </summary>
    public string ExportMultipleToJson(IEnumerable<TableLayoutDto> layouts)
    {
        if (layouts == null)
        {
            throw new ArgumentNullException(nameof(layouts));
        }

        var layoutList = layouts.ToList();
        var exportData = new MultipleLayoutsExportData
        {
            Metadata = new LayoutMetadata
            {
                Name = "Multiple Layouts Export",
                Description = $"Exported {layoutList.Count} layouts on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                Version = 1,
                ExportDate = DateTime.UtcNow,
                TableCount = layoutList.Sum(l => l.Tables.Count),
                Software = "Magidesk POS",
                SoftwareVersion = "1.0.0"
            },
            Layouts = layoutList
        };

        return JsonSerializer.Serialize(exportData, _jsonOptions);
    }

    /// <summary>
    /// Imports multiple layouts from a JSON file
    /// </summary>
    public List<TableLayoutDto> ImportMultipleFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON data cannot be null or empty.", nameof(json));
        }

        try
        {
            var exportData = JsonSerializer.Deserialize<MultipleLayoutsExportData>(json, _jsonOptions);
            
            if (exportData?.Layouts == null)
            {
                throw new InvalidOperationException("Invalid layout data in JSON file.");
            }

            var result = new List<TableLayoutDto>();
            foreach (var layout in exportData.Layouts)
            {
                ValidateImportedLayout(layout);
                result.Add(layout);
            }

            return result;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid JSON format.", ex);
        }
    }

    /// <summary>
    /// Creates a backup of all layouts
    /// </summary>
    public string CreateBackup(IEnumerable<TableLayoutDto> layouts)
    {
        var backupData = new LayoutBackupData
        {
            Metadata = new BackupMetadata
            {
                BackupType = "Full",
                Description = "Complete backup of all table layouts",
                BackupDate = DateTime.UtcNow,
                LayoutCount = layouts.Count(),
                TableCount = layouts.Sum(l => l.Tables.Count),
                Software = "Magidesk POS",
                SoftwareVersion = "1.0.0"
            },
            Layouts = layouts.ToList()
        };

        return JsonSerializer.Serialize(backupData, _jsonOptions);
    }

    /// <summary>
    /// Restores layouts from a backup file
    /// </summary>
    public List<TableLayoutDto> RestoreFromBackup(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Backup data cannot be null or empty.", nameof(json));
        }

        try
        {
            var backupData = JsonSerializer.Deserialize<LayoutBackupData>(json, _jsonOptions);
            
            if (backupData?.Layouts == null)
            {
                throw new InvalidOperationException("Invalid backup data.");
            }

            var result = new List<TableLayoutDto>();
            foreach (var layout in backupData.Layouts)
            {
                ValidateImportedLayout(layout);
                result.Add(layout);
            }

            return result;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid backup format.", ex);
        }
    }

    private void ValidateImportedLayout(TableLayoutDto layout)
    {
        if (string.IsNullOrWhiteSpace(layout.Name))
        {
            throw new InvalidOperationException("Layout name is required.");
        }

        if (layout.Tables == null || !layout.Tables.Any())
        {
            throw new InvalidOperationException("Layout must contain at least one table.");
        }

        // Validate each table
        foreach (var table in layout.Tables)
        {
            if (table.TableNumber <= 0)
            {
                throw new InvalidOperationException($"Table {table.TableNumber} has invalid table number.");
            }

            if (table.Capacity <= 0)
            {
                throw new InvalidOperationException($"Table {table.TableNumber} has invalid capacity.");
            }

            if (table.X < 0 || table.Y < 0)
            {
                throw new InvalidOperationException($"Table {table.TableNumber} has invalid position.");
            }
        }

        // Check for duplicate table numbers
        var duplicateNumbers = layout.Tables
            .GroupBy(t => t.TableNumber)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        if (duplicateNumbers.Any())
        {
            throw new InvalidOperationException($"Duplicate table numbers found: {string.Join(", ", duplicateNumbers)}");
        }
    }
}

// Export data models
public class TableLayoutExportData
{
    public LayoutMetadata Metadata { get; set; } = new();
    public TableLayoutDto Layout { get; set; } = new();
}

public class MultipleLayoutsExportData
{
    public LayoutMetadata Metadata { get; set; } = new();
    public List<TableLayoutDto> Layouts { get; set; } = new();
}

public class LayoutBackupData
{
    public BackupMetadata Metadata { get; set; } = new();
    public List<TableLayoutDto> Layouts { get; set; } = new();
}

public class LayoutMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime ExportDate { get; set; }
    public int TableCount { get; set; }
    public Guid? FloorId { get; set; }
    public string Software { get; set; } = string.Empty;
    public string SoftwareVersion { get; set; } = string.Empty;
}

public class BackupMetadata
{
    public string BackupType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime BackupDate { get; set; }
    public int LayoutCount { get; set; }
    public int TableCount { get; set; }
    public string Software { get; set; } = string.Empty;
    public string SoftwareVersion { get; set; } = string.Empty;
}
