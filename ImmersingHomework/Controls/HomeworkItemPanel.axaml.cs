using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ImmersingHomework.Models;

namespace ImmersingHomework.Controls;

public partial class HomeworkItemPanel : UserControl
{
    public static readonly StyledProperty<HomeworkItem> HomeworkItemProperty =
        AvaloniaProperty.Register<HomeworkItemPanel, HomeworkItem>(nameof(HomeworkItem));

    public HomeworkItem HomeworkItem
    {
        get => GetValue(HomeworkItemProperty);
        set => SetValue(HomeworkItemProperty, value);
    }

    public HomeworkItemPanel()
    {
        InitializeComponent();
        HomeworkItemProperty.Changed.AddClassHandler<HomeworkItemPanel>((panel, e) => panel.UpdatePanel());
    }

    private void UpdatePanel()
    {
        if (HomeworkItem == null) return;
        
        if (!string.IsNullOrEmpty(HomeworkItem.Content))
        {
            ContentText.Text = HomeworkItem.Content;
        }
        TagPanel.Children.Clear();
        
        var colors = new List<IBrush>
        {
            new SolidColorBrush(Color.FromRgb(220, 240, 255)), 
            new SolidColorBrush(Color.FromRgb(220, 255, 230)), 
            new SolidColorBrush(Color.FromRgb(255, 250, 220)), 
            new SolidColorBrush(Color.FromRgb(255, 230, 255)), 
            new SolidColorBrush(Color.FromRgb(255, 235, 230))
        };

        var tags = HomeworkItem.Tags ?? Enumerable.Empty<string>();
        for (int i = 0; i < tags.Count(); i++)
        {
            var tagName = tags.ElementAt(i);
            if (!string.IsNullOrEmpty(tagName))
            {
                var tag = new Tag
                {
                    TagName = tagName,
                    TagColor = colors[i % colors.Count]
                };
                TagPanel.Children.Add(tag);
            }
        }
    }
}