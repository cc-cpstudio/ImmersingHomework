using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImmersingHomework.Views;

namespace ImmersingHomework;

public partial class App : Application
{
    private MainWindow? _mainWindow;
    private FloatingButtonWindow? _floatingButtonWindow;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainWindow = new MainWindow();
            _floatingButtonWindow = new FloatingButtonWindow();
            
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