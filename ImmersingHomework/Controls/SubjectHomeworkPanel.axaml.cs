using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using ImmersingHomework.Models;

namespace ImmersingHomework.Controls;

public partial class SubjectHomeworkPanel : UserControl
{
    public string Subject { get; set; }
    public List<HomeworkItem>  HomeworkItems { get; set; }
    
    public SubjectHomeworkPanel()
    {
        InitializeComponent();
    }

    public void Refresh()
    {
        SubjectText.Text = Subject;
    }
}