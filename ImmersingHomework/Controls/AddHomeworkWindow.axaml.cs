using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ImmersingHomework.Models;

namespace ImmersingHomework.Controls;

public partial class AddHomeworkWindow : Window
{
    public HomeworkItem? Result { get; private set; }
    public bool IsDeleted { get; private set; }
    private readonly HomeworkItem? _existingItem;

    public AddHomeworkWindow()
    {
        InitializeComponent();
        Title = "ImmersingHomework - 添加作业项";
        DeleteButton.IsVisible = false;
    }

    public AddHomeworkWindow(HomeworkItem existingItem) : this()
    {
        _existingItem = existingItem;
        Title = "ImmersingHomework - 修改作业项";
        DeleteButton.IsVisible = true;
        
        this.AttachedToVisualTree += (_, _) =>
        {
            SubjectPicker.SetSelectedSubject(existingItem.Subject);
            ContentInput.Text = existingItem.Content;
            TagPicker.SetSelectedTags(existingItem.Tags);
        };
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Result = GetHomework();
        if (Result != null)
        {
            if (_existingItem != null)
            {
                Result = new HomeworkItem(Result.Subject, Result.Content, Result.Tags)
                {
                    Id = _existingItem.Id
                };
            }
            Close(true);
        }
        else
        {
            Close(false);
        }
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Result = null;
        Close(false);
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Result = null;
        IsDeleted = true;
        Close(true);
    }

    public HomeworkItem? GetHomework()
    {
        string? subject = SubjectPicker.GetSelectedSubject();
        string? content = ContentInput?.Text?.Trim();
        List<string> tags = TagPicker.GetSelectedTags();

        if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(content))
        {
            return null;
        }
        return new HomeworkItem(subject, content, tags);
    }
}
