using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ImmersingHomework.Controls;

public partial class SubjectPicker : UserControl
{
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
}