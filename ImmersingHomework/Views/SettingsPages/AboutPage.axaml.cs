using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Serilog;

namespace ImmersingHomework.Views.SettingsPages;

public partial class AboutPage : UserControl
{
    private readonly ILogger _logger = Log.ForContext<AboutPage>();
    public AboutPage()
    {
        _logger.Debug("AboutPage 初始化");
        InitializeComponent();
    }
}
