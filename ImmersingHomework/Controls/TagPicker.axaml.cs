using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using ImmersingHomework.Models;
using Serilog;

namespace ImmersingHomework.Controls;

public partial class TagPicker : UserControl
{
    private readonly ILogger _logger = Log.ForContext<TagPicker>();
    private readonly List<PickableTag> _pickableTags = [];

    public TagPicker()
    {
        InitializeComponent();

        this.AttachedToVisualTree += (sender, args) =>
        {
            LoadTags();
        };
    }

    private void LoadTags()
    {
        if (PickableTagPanel == null) return;

        PickableTagPanel.Children.Clear();
        _pickableTags.Clear();

        foreach (var tagModel in AppSettings.Instance.Tags)
        {
            var pickableTag = new PickableTag
            {
                TagName = tagModel.Name,
                TagColor = tagModel.Color
            };
            PickableTagPanel.Children.Add(pickableTag);
            _pickableTags.Add(pickableTag);
        }
    }

    public List<string> GetSelectedTags()
    {
        return _pickableTags
            .Where(tag => tag.IsChecked)
            .Select(tag => tag.TagName)
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();
    }

    public void SetSelectedTags(List<string> selectedTags)
    {
        foreach (var pickableTag in _pickableTags)
        {
            pickableTag.IsChecked = selectedTags.Contains(pickableTag.TagName);
        }
    }

    public void ClearSelection()
    {
        foreach (var pickableTag in _pickableTags)
        {
            pickableTag.IsChecked = false;
        }
    }
}