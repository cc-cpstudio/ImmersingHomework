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
        
        var defaultColors = new List<IBrush>
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
                IBrush tagColor = defaultColors[i % defaultColors.Count];
                
                // 在 AppSettings 中查找对应标签的颜色
                var tagModel = AppSettings.Instance.Tags.FirstOrDefault(t => t.Name == tagName);
                if (tagModel != null && tagModel.Color != null)
                {
                    tagColor = tagModel.Color;
                }
                
                var tag = new Tag
                {
                    TagName = tagName,
                    TagColor = tagColor
                };
                TagPanel.Children.Add(tag);
            }
        }
    }
}