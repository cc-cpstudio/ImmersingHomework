using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ImmersingHomework.Models;

namespace ImmersingHomework.Controls;

public partial class AddHomeworkWindow : Window
{
    public HomeworkItem? Result { get; private set; }

    public AddHomeworkWindow()
    {
        InitializeComponent();
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Result = GetHomework();
        if (Result != null)
        {
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
