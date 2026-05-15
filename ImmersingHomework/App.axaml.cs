using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImmersingHomework.Abstractions;
using ImmersingHomework.Services.Platforms;
using ImmersingHomework.Views;

namespace ImmersingHomework;

public partial class App : Application
{
    private MainWindow? _mainWindow;
    private FloatingButtonWindow? _floatingButtonWindow;
    private PlatformServiceBase? _platformService;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _platformService = CreatePlatformService();
            
            _mainWindow = new MainWindow();
            _floatingButtonWindow = new FloatingButtonWindow();
            
            if (_platformService != null)
            {
                _platformService.SetTopmost(_floatingButtonWindow);
                _platformService.DisableFocus(_floatingButtonWindow);
                _platformService.HideFromTaskbar(_floatingButtonWindow);
                _platformService.HideFromAltTab(_floatingButtonWindow);
            }
            
            desktop.MainWindow = _mainWindow;
            
            _mainWindow.WindowMinimized += MainWindow_WindowMinimized;
            _mainWindow.WindowActivated += MainWindow_WindowActivated;
            _mainWindow.WindowDeactivated += MainWindow_WindowDeactivated;
            _mainWindow.Closing += MainWindow_Closing;
            
            _floatingButtonWindow.FloatingButtonClicked += FloatingButtonWindow_FloatingButtonClicked;
            
            _mainWindow.Show();
            _floatingButtonWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private PlatformServiceBase CreatePlatformService()
    {
        if (OperatingSystem.IsWindows())
            return new WindowsPlatformService();
        if (OperatingSystem.IsMacOS())
            return new MacOSPlatformService();
        if (OperatingSystem.IsLinux())
        {
            var xdgSession = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
            if (!string.IsNullOrEmpty(xdgSession))
            {
                if (xdgSession.Equals("x11", StringComparison.OrdinalIgnoreCase))
                    return new X11PlatformService();
                if (xdgSession.Equals("wayland", StringComparison.OrdinalIgnoreCase))
                    return new WaylandPlatformService();
            }
            
            var waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            if (!string.IsNullOrEmpty(waylandDisplay))
                return new WaylandPlatformService();
            
            var display = Environment.GetEnvironmentVariable("DISPLAY");
            if (!string.IsNullOrEmpty(display))
                return new X11PlatformService();
            
            return new X11PlatformService(); // 默认使用 X11
        }
        throw new PlatformNotSupportedException();
    }

    private async void MainWindow_WindowMinimized(object? sender, EventArgs e)
    {
        ShowFloatingButton();
        // 等待一小段时间让浮窗显示完成，然后隐藏主窗口
        await Task.Delay(200);
        _mainWindow?.Hide();
    }

    private void MainWindow_WindowActivated(object? sender, EventArgs e)
    {
        HideFloatingButton();
    }

    private void MainWindow_WindowDeactivated(object? sender, EventArgs e)
    {
        if (_mainWindow != null && _mainWindow.WindowState == Avalonia.Controls.WindowState.Minimized)
        {
            ShowFloatingButton();
        }
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _floatingButtonWindow?.Close();
    }

    private void FloatingButtonWindow_FloatingButtonClicked(object? sender, EventArgs e)
    {
        ShowMainWindow();
    }

    private void ShowMainWindow()
    {
        if (_mainWindow != null)
        {
            _mainWindow.WindowState = Avalonia.Controls.WindowState.FullScreen;
            _mainWindow.Activate();
            _mainWindow.Show();
        }
    }

    private void ShowFloatingButton()
    {
        _floatingButtonWindow?.ShowWithAnimation();
    }

    private void HideFloatingButton()
    {
        _floatingButtonWindow?.HideWithAnimation();
    }
}