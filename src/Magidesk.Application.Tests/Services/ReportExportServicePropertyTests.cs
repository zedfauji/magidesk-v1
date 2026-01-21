using FsCheck;
using FsCheck.Xunit;
using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services.Reports;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Xunit;

namespace Magidesk.Application.Tests.Services;

/// <summary>
/// Property-based tests for ReportExportService.
/// Feature: reporting-export, Property 7: Export Format Integrity
/// Validates: Requirements 6.1, 6.3, 6.4
/// </summary>
public class ReportExportServicePropertyTests
{
    private readonly Mock<ILogger<ReportExportService>> _mockLogger;
    private readonly ReportExportService _exportService;

    public ReportExportServicePropertyTests()
    {
        _mockLogger = new Mock<ILogger<ReportExportService>>();
        _exportService = new ReportExportService(_mockLogger.Object);
    }

    /// <summary>
    /// Unit test: PDF export with valid template should return non-empty byte array.
    /// </summary>
    [Fact]
    public async Task PdfExport_WithValidTemplate_ReturnsNonEmptyBytes()
    {
        // Arrange
        var reportData = CreateSampleDailySalesReport();
        var templateName = "daily-sales";

        // Act
        var result = await _exportService.ExportToPdfAsync(reportData, templateName);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
        
        // Verify it starts with PDF header
        var header = Encoding.UTF8.GetString(result.Take(8).ToArray());
        Assert.StartsWith("%PDF-", header);
    }

    /// <summary>
    /// Unit test: Excel export with valid template should return non-empty byte array.
    /// </summary>
    [Fact]
    public async Task ExcelExport_WithValidTemplate_ReturnsNonEmptyBytes()
    {
        // Arrange
        var reportData = CreateSampleDailySalesReport();
        var templateName = "daily-sales";

        // Act
        var result = await _exportService.ExportToExcelAsync(reportData, templateName);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    /// <summary>
    /// Unit test: Export with invalid template should throw ArgumentException.
    /// </summary>
    [Fact]
    public async Task Export_WithInvalidTemplate_ThrowsArgumentException()
    {
        // Arrange
        var reportData = CreateSampleDailySalesReport();
        var invalidTemplateName = "non-existent-template";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _exportService.ExportToPdfAsync(reportData, invalidTemplateName));
        
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _exportService.ExportToExcelAsync(reportData, invalidTemplateName));
    }

    /// <summary>
    /// Unit test: Batch export with mixed valid and invalid requests.
    /// </summary>
    [Fact]
    public async Task BatchExport_WithMixedRequests_ReturnsPartialSuccess()
    {
        // Arrange
        var reportData = CreateSampleDailySalesReport();
        var requests = new[]
        {
            new ExportRequest(reportData, typeof(DailySalesReportDto), ExportFormat.Pdf, "daily-sales", "valid.pdf"),
            new ExportRequest(reportData, typeof(DailySalesReportDto), ExportFormat.Excel, "invalid-template", "invalid.xlsx"),
            new ExportRequest(reportData, typeof(DailySalesReportDto), ExportFormat.Pdf, "daily-sales", "valid2.pdf")
        };

        // Act
        var result = await _exportService.BatchExportAsync(requests);

        // Assert
        Assert.False(result.IsSuccess); // Should be false due to one invalid request
        Assert.Equal(3, result.Files.Count());
        Assert.Equal(2, result.Files.Count(f => f.IsSuccess)); // Two valid exports
        Assert.Equal(1, result.Files.Count(f => !f.IsSuccess)); // One invalid export
        Assert.Single(result.Errors);
    }

    /// <summary>
    /// Unit test: Export format validation with valid parameters.
    /// </summary>
    [Fact]
    public async Task ValidateExportFormat_WithValidParameters_ReturnsValid()
    {
        // Arrange
        var format = ExportFormat.Pdf;
        var templateName = "daily-sales";
        var reportType = typeof(DailySalesReportDto);

        // Act
        var result = await _exportService.ValidateExportFormatAsync(format, templateName, reportType);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.ValidationErrors);
    }

    /// <summary>
    /// Unit test: Export format validation with invalid template.
    /// </summary>
    [Fact]
    public async Task ValidateExportFormat_WithInvalidTemplate_ReturnsInvalid()
    {
        // Arrange
        var format = ExportFormat.Pdf;
        var invalidTemplateName = "non-existent";
        var reportType = typeof(DailySalesReportDto);

        // Act
        var result = await _exportService.ValidateExportFormatAsync(format, invalidTemplateName, reportType);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.ValidationErrors);
        Assert.Contains(result.ValidationErrors, e => e.Contains("not found"));
    }

    /// <summary>
    /// Property: For any valid report data and supported template, export should produce non-empty output.
    /// **Validates: Requirements 6.1, 6.3, 6.4**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property ExportIntegrity_ValidDataAndTemplate_ProducesNonEmptyOutput()
    {
        return Prop.ForAll(
            GenerateValidReportData(),
            GenerateSupportedTemplate(),
            GenerateExportFormat(),
            (reportData, templateName, format) =>
            {
                try
                {
                    var task = format switch
                    {
                        ExportFormat.Pdf => _exportService.ExportToPdfAsync(reportData, templateName),
                        ExportFormat.Excel => _exportService.ExportToExcelAsync(reportData, templateName),
                        _ => throw new ArgumentException($"Unsupported format: {format}")
                    };

                    var result = task.GetAwaiter().GetResult();

                    // Property: Export should always produce non-empty output for valid inputs
                    return result != null && result.Length > 0;
                }
                catch (Exception)
                {
                    // Valid inputs should not throw exceptions
                    return false;
                }
            });
    }

    /// <summary>
    /// Property: For any batch export request, the number of results should equal the number of requests.
    /// **Validates: Requirements 6.1, 6.4**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property BatchExportCompleteness_RequestCountEqualsResultCount()
    {
        return Prop.ForAll(
            GenerateBatchExportRequests(),
            (requests) =>
            {
                var task = _exportService.BatchExportAsync(requests);
                var result = task.GetAwaiter().GetResult();
                
                // Property: Number of results should always equal number of requests
                return result.Files.Count() == requests.Count();
            });
    }

    /// <summary>
    /// Property: For any export validation, invalid templates should always be rejected.
    /// **Validates: Requirements 6.1, 6.3**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property ValidationConsistency_InvalidTemplatesAlwaysRejected()
    {
        return Prop.ForAll(
            GenerateInvalidTemplate(),
            GenerateExportFormat(),
            GenerateSupportedReportType(),
            (invalidTemplate, format, reportType) =>
            {
                var task = _exportService.ValidateExportFormatAsync(format, invalidTemplate, reportType);
                var result = task.GetAwaiter().GetResult();
                
                // Property: Invalid templates should always result in validation failure
                return !result.IsValid && result.ValidationErrors.Any();
            });
    }

    /// <summary>
    /// Property: For any successful export, the output should maintain data type integrity.
    /// **Validates: Requirements 6.3, 6.4**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property DataTypeIntegrity_ExportMaintainsDataTypes()
    {
        return Prop.ForAll(
            GenerateValidReportData(),
            GenerateSupportedTemplate(),
            (reportData, templateName) =>
            {
                try
                {
                    // Test both PDF and Excel exports
                    var pdfTask = _exportService.ExportToPdfAsync(reportData, templateName);
                    var excelTask = _exportService.ExportToExcelAsync(reportData, templateName);
                    
                    var pdfResult = pdfTask.GetAwaiter().GetResult();
                    var excelResult = excelTask.GetAwaiter().GetResult();

                    // Property: Both exports should succeed and produce valid output
                    var pdfValid = pdfResult != null && pdfResult.Length > 0;
                    var excelValid = excelResult != null && excelResult.Length > 0;
                    
                    // For PDF, check it starts with PDF header
                    var pdfFormatValid = pdfResult.Length >= 4 && 
                        Encoding.UTF8.GetString(pdfResult.Take(4).ToArray()) == "%PDF";
                    
                    return pdfValid && excelValid && pdfFormatValid;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }

    /// <summary>
    /// Property: For any concurrent export requests, each should be processed independently.
    /// **Validates: Requirements 6.1, 6.4**
    /// </summary>
    [Property(MaxTest = 50)] // Reduced iterations for concurrent testing
    public Property ConcurrentExportIndependence_NoDataCorruption()
    {
        return Prop.ForAll(
            GenerateValidReportData(),
            GenerateSupportedTemplate(),
            (reportData, templateName) =>
            {
                try
                {
                    // Run multiple concurrent exports
                    var tasks = Enumerable.Range(0, 5).Select(_ => 
                        _exportService.ExportToPdfAsync(reportData, templateName)).ToArray();
                    
                    var results = Task.WhenAll(tasks).GetAwaiter().GetResult();
                    
                    // Property: All concurrent exports should succeed and produce identical results
                    var allSuccessful = results.All(r => r != null && r.Length > 0);
                    var allIdentical = results.Skip(1).All(r => r.SequenceEqual(results[0]));
                    
                    return allSuccessful && allIdentical;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }

    // Generator methods for property-based testing

    private static Arbitrary<DailySalesReportDto> GenerateValidReportData()
    {
        return Arb.From(Gen.Fresh(() => CreateSampleDailySalesReport()));
    }

    private static Arbitrary<string> GenerateSupportedTemplate()
    {
        var supportedTemplates = new[] { "daily-sales", "time-revenue", "shift-summary" };
        return Arb.From(Gen.Elements(supportedTemplates));
    }

    private static Arbitrary<string> GenerateInvalidTemplate()
    {
        var invalidTemplates = new[] { "invalid-template", "non-existent", "", "   ", "unknown-template" };
        return Arb.From(Gen.Elements(invalidTemplates));
    }

    private static Arbitrary<ExportFormat> GenerateExportFormat()
    {
        return Arb.From(Gen.Elements(ExportFormat.Pdf, ExportFormat.Excel));
    }

    private static Arbitrary<Type> GenerateSupportedReportType()
    {
        var supportedTypes = new[] { typeof(DailySalesReportDto), typeof(TimeRevenueReportDto), typeof(ShiftSummaryReportDto) };
        return Arb.From(Gen.Elements(supportedTypes));
    }

    private static Arbitrary<IEnumerable<ExportRequest>> GenerateBatchExportRequests()
    {
        return Arb.From(
            Gen.Choose(1, 5).SelectMany(count =>
                Gen.ArrayOf(count, Gen.Fresh(() => new ExportRequest(
                    CreateSampleDailySalesReport(),
                    typeof(DailySalesReportDto),
                    ExportFormat.Pdf,
                    "daily-sales",
                    $"report_{Guid.NewGuid()}.pdf"
                ))).Select(arr => arr.AsEnumerable())
            )
        );
    }

    private static DailySalesReportDto CreateSampleDailySalesReport()
    {
        return new DailySalesReportDto(
            Date: DateTime.Today,
            TotalSales: new Money(1000m, "USD"),
            TimeBasedSales: new Money(600m, "USD"),
            ProductSales: new Money(400m, "USD"),
            TotalTax: new Money(80m, "USD"),
            TotalGratuity: new Money(120m, "USD"),
            TransactionCount: 25,
            CustomerCount: 20,
            AverageTicketSize: 40m,
            HourlyBreakdown: new[]
            {
                new HourlySalesDto(9, new Money(100m, "USD"), 3, 3),
                new HourlySalesDto(10, new Money(150m, "USD"), 4, 4),
                new HourlySalesDto(11, new Money(200m, "USD"), 5, 5)
            },
            CategoryBreakdown: new[]
            {
                new CategorySalesDto("Food", new Money(300m, "USD"), 15, 30m),
                new CategorySalesDto("Beverages", new Money(100m, "USD"), 10, 10m)
            },
            PaymentBreakdown: new[]
            {
                new PaymentMethodSalesDto("Cash", new Money(400m, "USD"), 10, 40m),
                new PaymentMethodSalesDto("Card", new Money(600m, "USD"), 15, 60m)
            },
            TableBreakdown: new[]
            {
                new TableSalesDto(1, "Pool", new Money(200m, "USD"), new Money(100m, "USD"), 
                    new Money(300m, "USD"), TimeSpan.FromHours(4), 2),
                new TableSalesDto(2, "Snooker", new Money(400m, "USD"), new Money(300m, "USD"), 
                    new Money(700m, "USD"), TimeSpan.FromHours(6), 3)
            }
        );
    }
}