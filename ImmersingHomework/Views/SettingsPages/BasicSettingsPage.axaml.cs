using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Serilog;

namespace ImmersingHomework.Views.SettingsPages;

public partial class BasicSettingsPage : UserControl
{
    private readonly ILogger _logger = Log.ForContext<BasicSettingsPage>();
    public BasicSettingsPage()
    {
        InitializeComponent();
    }
}