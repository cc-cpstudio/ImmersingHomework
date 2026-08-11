using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ImmersingHomework.Controls;

public partial class HomeworkStorageManageDialogContent : UserControl
{
    public HomeworkStorageManageDialogContent()
    {
        InitializeComponent();
    }

    private void CalendarDatePicker_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DateOnly.FromDateTime(DateTime.Now).DayNumber -
            DateOnly.FromDateTime((DateTime)CalendarDatePicker.SelectedDate!).DayNumber < 3)
        {
            WarningTextBlock.Text = "保留的作业数据太少，请谨慎删除";
        }
    }
}