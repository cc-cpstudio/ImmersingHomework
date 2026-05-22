using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Serilog;

namespace ImmersingHomework.Controls;

public partial class PickableTag : UserControl
{
    private readonly ILogger _logger = Log.ForContext<PickableTag>();
    public static readonly StyledProperty<string> TagNameProperty =
        AvaloniaProperty.Register<PickableTag, string>(nameof(TagName));

    public static readonly StyledProperty<IBrush> TagColorProperty =
        AvaloniaProperty.Register<PickableTag, IBrush>(nameof(TagColor), new SolidColorBrush(Color.FromRgb(220, 240, 255)));

    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<PickableTag, bool>(nameof(IsChecked));

    public string TagName
    {
        get => GetValue(TagNameProperty);
        set => SetValue(TagNameProperty, value);
    }

    public IBrush TagColor
    {
        get => GetValue(TagColorProperty);
        set => SetValue(TagColorProperty, value);
    }

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    private IBrush? _originalBackground;
    private IBrush? _originalForeground;

    public PickableTag()
    {
        InitializeComponent();

        IsCheckedProperty.Changed.AddClassHandler<PickableTag>((tag, e) => tag.UpdateVisual());
        TagNameProperty.Changed.AddClassHandler<PickableTag>((tag, e) => tag.UpdateTagName());
        TagColorProperty.Changed.AddClassHandler<PickableTag>((tag, e) => tag.UpdateVisual());

        TagToggleButton.IsCheckedChanged += (sender, args) =>
        {
            IsChecked = TagToggleButton.IsChecked ?? false;
        };

        this.AttachedToVisualTree += (sender, args) =>
        {
            UpdateTagName();
            UpdateVisual();
        };
    }

    private void UpdateTagName()
    {
        if (TagText != null && !string.IsNullOrEmpty(TagName))
        {
            TagText.Text = TagName;
        }
    }

    private void UpdateVisual()
    {
        if (TagToggleButton == null || TagText == null) return;

        if (IsChecked)
        {
            if (_originalBackground == null)
            {
                _originalBackground = TagToggleButton.Background;
            }
            if (_originalForeground == null)
            {
                _originalForeground = TagText.Foreground;
            }

            TagText.Foreground = new SolidColorBrush(Colors.White);
            TagToggleButton.Background = LightenBrush(TagColor, 0.3f);
        }
        else
        {
            if (_originalForeground != null)
            {
                TagText.Foreground = new SolidColorBrush(Colors.Black);
            }
            TagToggleButton.Background = TagColor;
        }
    }

    private IBrush LightenBrush(IBrush brush, float amount)
    {
        if (brush is SolidColorBrush solidColorBrush)
        {
            var color = solidColorBrush.Color;
            var r = (byte)Math.Min(255, color.R + (255 - color.R) * amount);
            var g = (byte)Math.Min(255, color.G + (255 - color.G) * amount);
            var b = (byte)Math.Min(255, color.B + (255 - color.B) * amount);
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }
        return brush;
    }
}