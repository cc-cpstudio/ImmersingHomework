using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using ImmersingHomework.Models;
using Serilog;

namespace ImmersingHomework.Views.SettingsPages;

public partial class SubjectSettingsPage : UserControl
{
    private readonly ILogger _logger = Log.ForContext<SubjectSettingsPage>();
    public SubjectSettingsPage()
    {
        InitializeComponent();
        this.AttachedToVisualTree += (_, _) => Refresh();
    }

    public void Refresh()
    {
        SubjectPanel.Children.Clear();
        foreach (var subject in AppSettings.Instance.Subjects)
        {
            var button = new Button { Content = subject };
            button.Click += async (s, e) => await OnSubjectButtonClick(subject);
            SubjectPanel.Children.Add(button);
        }
    }

    private async System.Threading.Tasks.Task OnSubjectButtonClick(string subjectName)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        if (window == null)
            return;

        var dialog = new FAContentDialog()
        {
            Title = "删除科目",
            Content = $"确定要删除科目 \"{subjectName}\" 吗？",
            PrimaryButtonText = "删除",
            CloseButtonText = "取消"
        };

        var result = await dialog.ShowAsync(window);

        if (result == FAContentDialogResult.Primary)
        {
            AppSettings.Instance.Subjects.Remove(subjectName);
            Refresh();
        }
    }

    private async void AddSubjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        if (window == null)
            return;

        var textBox = new TextBox
        {
            PlaceholderText = "请输入科目名称",
            Width = 300
        };

        var dialog = new FAContentDialog()
        {
            Title = "添加科目",
            Content = textBox,
            PrimaryButtonText = "添加",
            CloseButtonText = "取消"
        };

        var result = await dialog.ShowAsync(window);

        if (result == FAContentDialogResult.Primary)
        {
            var subjectName = textBox.Text?.Trim();
            if (string.IsNullOrEmpty(subjectName))
            {
                return;
            }

            if (AppSettings.Instance.Subjects.Contains(subjectName))
            {
                var errorDialog = new FAContentDialog()
                {
                    Title = "错误",
                    Content = "该科目已存在，请输入其他名称。",
                    CloseButtonText = "确定"
                };
                await errorDialog.ShowAsync(window);
                return;
            }

            AppSettings.Instance.Subjects.Add(subjectName);
            Refresh();
        }
    }
}
