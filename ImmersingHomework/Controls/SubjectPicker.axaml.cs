using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Serilog;

namespace ImmersingHomework.Controls;

public partial class SubjectPicker : UserControl
{
    private readonly ILogger _logger = Log.ForContext<SubjectPicker>();
    public SubjectPicker()
    {
        InitializeComponent();

        List<string> subjects = ["语文", "数学", "英语", "物理"];
        foreach (var subject in subjects)
        {
            SubjectPanel.Children.Add(new RadioButton()
            {
                Content = subject,
                GroupName = "Subjects"
            });
        }
    }

    public string? GetSelectedSubject()
    {
        foreach (var subjectRadioButton in SubjectPanel.Children)
        {
            if (subjectRadioButton is not RadioButton subject) continue;
            if (subject is { IsChecked: true, Content: not null })
            {
                return subject.Content.ToString();
            }
        }
        return null;
    }

    public void SetSelectedSubject(string? subjectName)
    {
        foreach (var subjectRadioButton in SubjectPanel.Children)
        {
            if (subjectRadioButton is not RadioButton subject) continue;
            if (subject.Content?.ToString() == subjectName)
            {
                subject.IsChecked = true;
                break;
            }
        }
    }
}