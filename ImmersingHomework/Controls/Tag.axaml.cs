using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ImmersingHomework.Controls;

public partial class Tag : UserControl
{
    public static readonly StyledProperty<string> TagNameProperty =
        AvaloniaProperty.Register<Tag, string>(nameof(TagName));

    public static readonly StyledProperty<IBrush> TagColorProperty =
        AvaloniaProperty.Register<Tag, IBrush>(nameof(TagColor), new SolidColorBrush(Color.FromRgb(220, 240, 255)));

    public string TagName
    {
        get => GetValue(TagNameProperty);
        set 
        { 
            SetValue(TagNameProperty, value);
            if (TagText != null)
                TagText.Text = value;
        }
    }

    public IBrush TagColor
    {
        get => GetValue(TagColorProperty);
        set 
        { 
            SetValue(TagColorProperty, value);
            if (TagBorder != null && value != null)
                TagBorder.Background = value;
        }
    }

    public Tag()
    {
        InitializeComponent();
    }
}