using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using ImmersingHomework.Controls;
using ImmersingHomework.Enums;
using ImmersingHomework.Models;
using ImmersingHomework.Services;
using Serilog;

namespace ImmersingHomework.Views;

public partial class MainWindow : Window
{
    private readonly ILogger _logger = Log.ForContext<MainWindow>();
    private Timer _hitokotoTimer;
    
    private event Action<DateOnly> DateChanged;
    
    public event EventHandler? WindowMinimized;
    public event EventHandler? WindowActivated;
    public event EventHandler? WindowDeactivated;

    public DateOnly Date
    {
        get;
        set
        {
            field = value;
            DateChanged?.Invoke(field);
        }
    }

    private readonly HomeworkStorageService _storageService;
    private Bitmap? _clipboardBitmap;

    public MainWindow()
    {
        _logger.Information("MainWindow 初始化开始");
        InitializeComponent();
        WindowState = WindowState.FullScreen;
        
        _storageService = new HomeworkStorageService();
        DateChanged += UpdateDateText;
        DateChanged += (date) => HomeworkPanel.Date = date;
        Date = DateOnly.FromDateTime(DateTime.Now);
        
        CalendarPopup.PlacementTarget = DateButton;
        
        this.Activated += (s, e) => 
        {
            _logger.Debug("窗口激活");
            WindowActivated?.Invoke(this, EventArgs.Empty);
        };
        this.Deactivated += (s, e) => 
        {
            _logger.Debug("窗口失活");
            WindowDeactivated?.Invoke(this, EventArgs.Empty);
        };

        if (AppSettings.Instance.HitokotoDisplayMode.Value is not HitokotoDisplayMode.Hide)
            SetupHitokotoTimer();

        HomeworkPanel.Refresh();
        _logger.Information("MainWindow 初始化完成");
    }

    protected void SetupHitokotoTimer()
    {
        _logger.Debug("初始化 Hitokoto 定时器，间隔 {Span} 秒", AppSettings.Instance.HitokotoRefreshTimeSpan);
        _hitokotoTimer = new Timer(AppSettings.Instance.HitokotoRefreshTimeSpan.Value * 1000);
        AppSettings.Instance.HitokotoRefreshTimeSpan.ValueChanged += UpdateHitokotoTimerInterval;
        _hitokotoTimer.Elapsed += async (s, e) =>
        {
            _logger.Debug("Hitokoto 定时器触发，开始获取新的 Hitokoto");
            HitokotoService.Hitokoto? hitokoto = await HitokotoService.GetHitokoto();
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (hitokoto is null)
                {
                    _logger.Debug("获取到的 Hitokoto 为空，使用默认文本");
                    Hitokoto.Text = AppSettings.Instance.HitokotoDisplayMode.Value switch
                    {
                        HitokotoDisplayMode.Hide => "",
                        HitokotoDisplayMode.Content => "咕咕嘎嘎！",
                        HitokotoDisplayMode.ContentAndAuthor => "咕咕嘎嘎！ —— programmer_cc"
                    };
                }
                else
                {
                    _logger.Debug("更新 Hitokoto 显示: {Sentence} —— {Author}", hitokoto.Value.Sentence, hitokoto.Value.Author);
                    Hitokoto.Text = AppSettings.Instance.HitokotoDisplayMode.Value switch
                    {
                        HitokotoDisplayMode.Hide => "",
                        HitokotoDisplayMode.Content => hitokoto.Value.Sentence,
                        HitokotoDisplayMode.ContentAndAuthor => $"{hitokoto.Value.Sentence} —— {hitokoto.Value.Author}"
                    };
                }
            });
        };
        _hitokotoTimer.AutoReset = true;
        Closing += (s, e) =>
        {
            _logger.Debug("窗口关闭，停止并释放 Hitokoto 定时器");
            _hitokotoTimer.Stop();
            _hitokotoTimer.Dispose();
        };
        _hitokotoTimer.Start();
        _logger.Debug("Hitokoto 定时器已启动");
    }

    private void UpdateHitokotoTimerInterval(int span)
    {
        _hitokotoTimer.Interval = span * 1000;
    }
    
    public void UpdateDateText(DateOnly date)
    {
        DateText.Text = $"{ Date.Month }月{ Date.Day }日";
    }

    private void DateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CalendarPopup.IsOpen = true;
    }

    private void Calendar_OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        Date = Calendar.SelectedDate.HasValue
            ? DateOnly.FromDateTime(Calendar.SelectedDate.Value)
            : Date;
    }

    private async void AddHomeworkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _logger.Information("用户点击了添加作业按钮");
        var control = new AddHomeworkWindow();
        var dialog = new FAContentDialog()
        {
            Title = control.Title,
            Content = control,
            PrimaryButtonText = "确定",
            CloseButtonText = "取消"
        };
        
        control.SetDialog(dialog);
        dialog.PrimaryButtonClick += (s, args) => control.OnPrimaryButtonClick(args);
        
        var result = await dialog.ShowAsync(this);
        
        if (result == FAContentDialogResult.Primary && control.Result != null)
        {
            _logger.Information("用户确认添加新作业");
            var currentHomework = _storageService.Load(Date) ?? new Homework(Date, []);
            currentHomework.AddHomeworkItem(control.Result);
            _storageService.Save(currentHomework);
            HomeworkPanel.Refresh();
            _logger.Information("作业已保存");
        }
    }

    private void MinimizeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        WindowMinimized?.Invoke(this, EventArgs.Empty);
    }

    private async void ShareButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var homework = _storageService.Load(Date);
        if (homework is null) return;

        ExportFormat? exportFormat = null;
        var formatPanel = new StackPanel { Spacing = 8, Margin = new Avalonia.Thickness(0, 8, 0, 0) };
        var imageBtn = new Button { Content = "导出为图片", HorizontalAlignment = HorizontalAlignment.Stretch };
        var pdfBtn = new Button { Content = "导出为 PDF", HorizontalAlignment = HorizontalAlignment.Stretch };
        var qrBtn = new Button { Content = "通过二维码导出到其它设备", HorizontalAlignment = HorizontalAlignment.Stretch };
        var formatDialog = new FAContentDialog()
        {
            Title = "选择导出格式",
            Content = formatPanel,
            CloseButtonText = "取消"
        };
        imageBtn.Click += (_, _) => { exportFormat = ExportFormat.Image; formatDialog.Hide(); };
        pdfBtn.Click += (_, _) => { exportFormat = ExportFormat.Pdf; formatDialog.Hide(); };
        qrBtn.Click += (_, _) => { exportFormat = ExportFormat.QrCode; formatDialog.Hide(); };
        formatDialog.CloseButtonClick += (_, _) => { formatDialog.Hide(); };
        formatPanel.Children.Add(new TextBlock { Text = "请选择要导出的文件格式：" });
        formatPanel.Children.Add(imageBtn);
        formatPanel.Children.Add(pdfBtn);
        formatPanel.Children.Add(qrBtn);
        await formatDialog.ShowAsync(this);

        if (exportFormat is null) return;

        if (exportFormat == ExportFormat.Image)
        {
            var outputPath = $"Outputs/{Date:yyyy-MM-dd}_{DateTime.Now:HH-mm-ss}.png";
            HomeworkImageService.HomeworkToImage(homework, outputPath);
            await ShowExportResultDialog(outputPath, isImage: true);
        }
        else if (exportFormat == ExportFormat.Pdf)
        {
            var outputPath = $"Outputs/{Date:yyyy-MM-dd}_{DateTime.Now:HH-mm-ss}.pdf";
            var pdfService = new UafPdfService();
            pdfService.InitializeFonts();
            var pdfBytes = pdfService.GeneratePdfFromHomework(homework);
            var fullPath = Path.GetFullPath(outputPath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            await File.WriteAllBytesAsync(fullPath, pdfBytes);
            await ShowExportResultDialog(outputPath, isImage: false);
        }
        else if (exportFormat == ExportFormat.QrCode)
        {
        }
    }

    private async Task ShowExportResultDialog(string outputPath, bool isImage)
    {
        var fullPath = Path.GetFullPath(outputPath);
        var dialog = new FAContentDialog()
        {
            Title = "作业分享",
            Content = $"今日作业已保存到 {fullPath}，请自行查看或点击复制{(isImage ? "图片" : "文件路径")}。",
            PrimaryButtonText = "打开",
            SecondaryButtonText = "复制",
            CloseButtonText = "关闭"
        };
        dialog.PrimaryButtonClick += (_, _) =>
        {
            try
            {
                if (OperatingSystem.IsLinux())
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = fullPath,
                        UseShellExecute = false
                    });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "open",
                        Arguments = fullPath,
                        UseShellExecute = false
                    });
                }
                else
                {
                    Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "打开文件失败: {Path}", fullPath);
            }
            dialog.Hide();
        };
        if (isImage)
        {
            dialog.SecondaryButtonClick += async (_, _) =>
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard is null) return;
                _clipboardBitmap?.Dispose();
                _clipboardBitmap = new Bitmap(outputPath);
                await clipboard.SetBitmapAsync(_clipboardBitmap);
                await clipboard.FlushAsync();
                dialog.Hide();
            };
        }
        else
        {
            dialog.SecondaryButtonClick += async (_, _) =>
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard is not null)
                {
                    await clipboard.SetTextAsync(fullPath);
                }
                dialog.Hide();
            };
        }
        dialog.CloseButtonClick += (_, _) => { dialog.Hide(); };
        await dialog.ShowAsync(this);
    }
}