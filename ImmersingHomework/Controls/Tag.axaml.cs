using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace ImmersingHomework.Controls;

public partial class Tag : UserControl
{
    public string TagName { get; set; }
    public IBrush TagColor { get; set; }
    
    public Tag()
    {
        InitializeComponent();
        TagBorder.Background = TagColor;
        TagText.Text = TagName;
    }
}