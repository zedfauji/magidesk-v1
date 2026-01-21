using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace Magidesk.Presentation.Controls;

public sealed partial class SessionTimerControl : UserControl
{
    private DispatcherTimer? _timer;
    private TimeSpan _elapsedTime;
    private string _formattedTime = "00:00:00";
    private Brush _backgroundBrush;

    // Dependency Properties
    public static readonly DependencyProperty SessionStartTimeProperty =
        DependencyProperty.Register(
            nameof(SessionStartTime),
            typeof(DateTime?),
            typeof(SessionTimerControl),
            new PropertyMetadata(null, OnSessionStartTimeChanged));

    public static readonly DependencyProperty IsPausedProperty =
        DependencyProperty.Register(
            nameof(IsPaused),
            typeof(bool),
            typeof(SessionTimerControl),
            new PropertyMetadata(false, OnIsPausedChanged));

    public DateTime? SessionStartTime
    {
        get => (DateTime?)GetValue(SessionStartTimeProperty);
        set => SetValue(SessionStartTimeProperty, value);
    }

    public bool IsPaused
    {
        get => (bool)GetValue(IsPausedProperty);
        set => SetValue(IsPausedProperty, value);
    }

    public TimeSpan ElapsedTime
    {
        get => _elapsedTime;
        private set
        {
            _elapsedTime = value;
            UpdateFormattedTime();
            UpdateBackgroundBrush();
        }
    }

    public string FormattedTime
    {
        get => _formattedTime;
        private set
        {
            if (_formattedTime != value)
            {
                _formattedTime = value;
                Bindings.Update();
            }
        }
    }

    public Brush BackgroundBrush
    {
        get => _backgroundBrush;
        private set
        {
            if (_backgroundBrush != value)
            {
                _backgroundBrush = value;
                Bindings.Update();
            }
        }
    }

    public SessionTimerControl()
    {
        this.InitializeComponent();
        
        // Initialize with default green background
        _backgroundBrush = new SolidColorBrush(Color.FromArgb(255, 16, 124, 16)); // Green
        
        // Initialize timer
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += UpdateTime;
        
        this.Loaded += SessionTimerControl_Loaded;
        this.Unloaded += SessionTimerControl_Unloaded;
    }

    private void SessionTimerControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (SessionStartTime.HasValue && !IsPaused)
        {
            _timer?.Start();
            UpdateTime(null, null);
        }
    }

    private void SessionTimerControl_Unloaded(object sender, RoutedEventArgs e)
    {
        _timer?.Stop();
    }

    private static void OnSessionStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SessionTimerControl control)
        {
            control.UpdateTime(null, null);
            
            if (control.SessionStartTime.HasValue && !control.IsPaused)
            {
                control._timer?.Start();
            }
            else
            {
                control._timer?.Stop();
            }
        }
    }

    private static void OnIsPausedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SessionTimerControl control)
        {
            if (control.IsPaused)
            {
                control._timer?.Stop();
                control.FormattedTime = "PAUSED";
            }
            else if (control.SessionStartTime.HasValue)
            {
                control._timer?.Start();
                control.UpdateTime(null, null);
            }
        }
    }

    private void UpdateTime(object? sender, object? e)
    {
        if (IsPaused)
        {
            FormattedTime = "PAUSED";
            return;
        }

        if (!SessionStartTime.HasValue)
        {
            ElapsedTime = TimeSpan.Zero;
            return;
        }

        ElapsedTime = DateTime.Now - SessionStartTime.Value;
    }

    private void UpdateFormattedTime()
    {
        if (IsPaused)
        {
            FormattedTime = "PAUSED";
            return;
        }

        var elapsed = ElapsedTime;
        
        // Format: "1d 02:15:30" for sessions exceeding 24 hours
        if (elapsed.TotalDays >= 1)
        {
            var days = (int)elapsed.TotalDays;
            var hours = elapsed.Hours;
            var minutes = elapsed.Minutes;
            var seconds = elapsed.Seconds;
            FormattedTime = $"{days}d {hours:D2}:{minutes:D2}:{seconds:D2}";
        }
        else
        {
            // Format: "HH:MM:SS" for sessions under 24 hours
            FormattedTime = $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
        }
    }

    private void UpdateBackgroundBrush()
    {
        var totalMinutes = ElapsedTime.TotalMinutes;
        
        // Threshold-based color changes:
        // Green: < 50 minutes
        // Yellow: 50-55 minutes
        // Red: >= 55 minutes
        if (totalMinutes >= 55)
        {
            // Red for sessions >= 55 minutes
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 196, 43, 28)); // Red
        }
        else if (totalMinutes >= 50)
        {
            // Yellow for sessions between 50-55 minutes
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 202, 160, 0)); // Yellow
        }
        else
        {
            // Green for sessions < 50 minutes
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 16, 124, 16)); // Green
        }
    }
}
