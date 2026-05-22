using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using ImmersingHomework.Models;
using Serilog;

namespace ImmersingHomework.Controls;

public partial class SubjectHomeworkPanel : UserControl
{
    private readonly ILogger _logger = Log.ForContext<SubjectHomeworkPanel>();
    public static readonly StyledProperty<string> SubjectProperty =
        AvaloniaProperty.Register<SubjectHomeworkPanel, string>(nameof(Subject));

    public static readonly StyledProperty<List<HomeworkItem>> HomeworkItemsProperty =
        AvaloniaProperty.Register<SubjectHomeworkPanel, List<HomeworkItem>>(nameof(HomeworkItems), new List<HomeworkItem>());

    public string Subject
    {
        get => GetValue(SubjectProperty);
        set => SetValue(SubjectProperty, value);
    }

    public List<HomeworkItem> HomeworkItems
    {
        get => GetValue(HomeworkItemsProperty);
        set => SetValue(HomeworkItemsProperty, value);
    }

    public event Action<HomeworkItem>? EditRequested;

    public SubjectHomeworkPanel()
    {
        InitializeComponent();
        SubjectProperty.Changed.AddClassHandler<SubjectHomeworkPanel>((panel, e) => panel.Refresh());
        HomeworkItemsProperty.Changed.AddClassHandler<SubjectHomeworkPanel>((panel, e) => panel.Refresh());
    }

    public void Refresh()
    {
        if (!string.IsNullOrEmpty(Subject))
        {
            SubjectText.Text = Subject;
        }
        HomeworkItemPanels.Children.Clear();

        var items = HomeworkItems ?? Enumerable.Empty<HomeworkItem>();
        foreach (var item in items)
        {
            if (item != null)
            {
                var itemPanel = new HomeworkItemPanel
                {
                    HomeworkItem = item
                };
                itemPanel.EditRequested += (homeworkItem) => EditRequested?.Invoke(homeworkItem);
                HomeworkItemPanels.Children.Add(itemPanel);
            }
        }
    }
}