using System;
using System.Collections.Generic;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using ImmersingHomework.Models;
using Serilog;

namespace ImmersingHomework.Controls;

public partial class AddHomeworkWindow : UserControl
{
    private readonly ILogger _logger = Log.ForContext<AddHomeworkWindow>();
    public HomeworkItem? Result { get; private set; }
    public bool IsDeleted { get; private set; }
    private readonly HomeworkItem? _existingItem;
    private FAContentDialog? _dialog;

    public string Title { get; private set; }
    public string? SecondaryButtonText { get; private set; }

    public AddHomeworkWindow()
    {
        InitializeComponent();
        Title = "添加作业项";
        SecondaryButtonText = null;
    }

    public AddHomeworkWindow(HomeworkItem existingItem) : this()
    {
        _existingItem = existingItem;
        Title = "修改作业项";
        SecondaryButtonText = "删除";
        
        this.AttachedToVisualTree += (_, _) =>
        {
            SubjectPicker.SetSelectedSubject(existingItem.Subject);
            ContentInput.Text = existingItem.Content;
            TagPicker.SetSelectedTags(existingItem.Tags);
        };
    }

    public void SetDialog(FAContentDialog dialog)
    {
        _dialog = dialog;
    }

    public void OnPrimaryButtonClick(FAContentDialogButtonClickEventArgs args)
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
            IsDeleted = false;
        }
        else
        {
            args.Cancel = true;
        }
    }

    public void OnSecondaryButtonClick()
    {
        Result = null;
        IsDeleted = true;
    }

    public void OnCloseButtonClick()
    {
        Result = null;
        IsDeleted = false;
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
