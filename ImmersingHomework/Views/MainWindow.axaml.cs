using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Controls;
using ImmersingHomework.Controls;
using ImmersingHomework.Models;
using ImmersingHomework.Services;

namespace ImmersingHomework.Views;

public partial class MainWindow : Window
{
    private DateOnly _date;
    
    private event Action<DateOnly> DateChanged;
    
    public event EventHandler? WindowMinimized;
    public event EventHandler? WindowActivated;
    public event EventHandler? WindowDeactivated;
    
    public DateOnly Date
    {
        get => _date;
        set
        {
            _date = value;
            DateChanged?.Invoke(_date);
        }
    }
    
    private readonly HomeworkStorageService _storageService;

    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.FullScreen;
        
        _storageService = new HomeworkStorageService();
        DateChanged += UpdateDateText;
        DateChanged += (date) => HomeworkPanel.Date = date;
        Date = DateOnly.FromDateTime(DateTime.Now);
        
        CalendarPopup.PlacementTarget = DateButton;
        
        this.Activated += (s, e) => WindowActivated?.Invoke(this, EventArgs.Empty);
        this.Deactivated += (s, e) => WindowDeactivated?.Invoke(this, EventArgs.Empty);
        
        HomeworkPanel.Refresh();
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

    private async void AddHomeworkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var control = new AddHomeworkWindow();
        var dialog = new FAContentDialog()
        {
            Title = control.Title,
            Content = control,
            PrimaryButtonText = "确定",
            CloseButtonText = "取消"
        };
        
        control.SetDialog(dialog);
        dialog.PrimaryButtonClick += (s, args) => control.OnPrimaryButtonClick(args);
        
        var result = await dialog.ShowAsync(this);
        
        if (result == FAContentDialogResult.Primary && control.Result != null)
        {
            var currentHomework = _storageService.Load(Date) ?? new Homework(Date, []);
            currentHomework.AddHomeworkItem(control.Result);
            _storageService.Save(currentHomework);
            HomeworkPanel.Refresh();
        }
    }

    private void MinimizeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        WindowMinimized?.Invoke(this, EventArgs.Empty);
    }
}