using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class TableDesignerTestPage : Page
{
    private TableDesignerViewModel _viewModel;
    private int _passedTests = 0;
    private int _failedTests = 0;
    private int _totalTests = 0;

    public TableDesignerTestPage()
    {
        this.InitializeComponent();
        _viewModel = App.Services.GetRequiredService<TableDesignerViewModel>();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Navigate back to previous page
    }

    private async void RunAllTests_Click(object sender, RoutedEventArgs e)
    {
        await RunAllTestsAsync();
    }

    private async void TestBasicWorkflow_Click(object sender, RoutedEventArgs e)
    {
        await RunTestAsync("Basic Workflow", async () => await _viewModel.TestBasicDesignerWorkflow());
    }

    private async void TestDragDrop_Click(object sender, RoutedEventArgs e)
    {
        await RunTestAsync("Drag & Drop", async () => await _viewModel.TestDragAndDropFunctionality());
    }

    private async void TestShapes_Click(object sender, RoutedEventArgs e)
    {
        await RunTestAsync("Shape Selection", async () => await _viewModel.TestShapeSelection());
    }

    private async Task RunAllTestsAsync()
    {
        ResetTestResults();
        
        StatusText.Text = "Running all tests...";
        TestProgress.Value = 0;

        var tests = new[]
        {
            new { Name = "Basic Workflow", Test = (Func<Task<bool>>)(() => _viewModel.TestBasicDesignerWorkflow()) },
            new { Name = "Drag & Drop", Test = (Func<Task<bool>>)(() => _viewModel.TestDragAndDropFunctionality()) },
            new { Name = "Shape Selection", Test = (Func<Task<bool>>)(() => _viewModel.TestShapeSelection()) }
        };

        for (int i = 0; i < tests.Length; i++)
        {
            var test = tests[i];
            await RunTestAsync(test.Name, test.Test);
            
            TestProgress.Value = ((i + 1) * 100) / tests.Length;
            await Task.Delay(500); // Small delay for visual feedback
        }

        StatusText.Text = $"All tests completed. Passed: {_passedTests}, Failed: {_failedTests}";
    }

    private async Task RunTestAsync(string testName, Func<Task<bool>> testFunc)
    {
        try
        {
            StatusBarText.Text = $"Running {testName}...";
            UpdateResults($"[{DateTime.Now:HH:mm:ss}] Starting {testName} test...\r\n");

            var result = await testFunc();
            
            if (result)
            {
                _passedTests++;
                UpdateResults($"[{DateTime.Now:HH:mm:ss}] ✓ {testName} PASSED\r\n");
            }
            else
            {
                _failedTests++;
                UpdateResults($"[{DateTime.Now:HH:mm:ss}] ✗ {testName} FAILED\r\n");
            }
        }
        catch (Exception ex)
        {
            _failedTests++;
            UpdateResults($"[{DateTime.Now:HH:mm:ss}] ✗ {testName} ERROR: {ex.Message}\r\n");
        }
        finally
        {
            _totalTests++;
            UpdateStatistics();
        }
    }

    private void ResetTestResults()
    {
        _passedTests = 0;
        _failedTests = 0;
        _totalTests = 0;
        ResultsText.Text = string.Empty;
        UpdateStatistics();
    }

    private void UpdateResults(string message)
    {
        ResultsText.Text += message;
        var scrollViewer = ResultsText.Parent as ScrollViewer;
        scrollViewer?.ChangeView(0, double.MaxValue, null);
    }

    private void UpdateStatistics()
    {
        PassedCount.Text = _passedTests.ToString();
        FailedCount.Text = _failedTests.ToString();
        TotalCount.Text = _totalTests.ToString();
    }
}
