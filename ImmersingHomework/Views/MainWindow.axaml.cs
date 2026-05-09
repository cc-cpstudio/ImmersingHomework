using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ImmersingHomework.Views;

public partial class MainWindow : Window
{
    private DateOnly _date;
    
    private event Action<DateOnly> DateChanged;
    
    public DateOnly Date
    {
        get => _date;
        set
        {
            _date = value;
            DateChanged?.Invoke(_date);
        }
    }
    
    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.FullScreen;
        
        DateChanged += UpdateDateText;
        Date = DateOnly.FromDateTime(DateTime.Now);
        
        CalendarPopup.PlacementTarget = DateButton;
    }

    public void UpdateDateText(DateOnly date)
    {
        DateText.Text = $"{ Date.Month }月{ Date.Day }日";
    }

    private void DateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CalendarPopup.IsOpen = true;
    }

    private void Calendar_OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        Date = Calendar.SelectedDate.HasValue
            ? DateOnly.FromDateTime(Calendar.SelectedDate.Value)
            : Date;
    }
}