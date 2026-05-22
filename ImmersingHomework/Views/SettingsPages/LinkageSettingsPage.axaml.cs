using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Serilog;

namespace ImmersingHomework.Views.SettingsPages;

public partial class LinkageSettingsPage : UserControl
{
    private readonly ILogger _logger = Log.ForContext<LinkageSettingsPage>();
    public LinkageSettingsPage()
    {
        InitializeComponent();
    }
}