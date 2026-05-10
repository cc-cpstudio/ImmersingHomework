using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ImmersingHomework.Models;
using ImmersingHomework.Services;

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
        
        CreateSampleData();
    }

    private void CreateSampleData()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        
        var homeworkItems = new List<HomeworkItem>
        {
            new HomeworkItem("数学", "完成练习册第15-20页", new List<string> { "习题", "重要" }),
            new HomeworkItem("数学", "复习第三章知识点", new List<string> { "复习", "重点" }),
            new HomeworkItem("语文", "写一篇关于春天的作文", new List<string> { "作文", "周末" }),
            new HomeworkItem("英语", "背诵单词表Unit5", new List<string> { "背诵", "测试" }),
            new HomeworkItem("物理", "完成实验报告", new List<string> { "实验", "报告" })
        };

        var homework = new Homework(today, homeworkItems);
        _storageService.Save(homework);
        
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
}