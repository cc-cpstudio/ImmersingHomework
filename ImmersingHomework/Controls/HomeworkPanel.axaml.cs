using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ImmersingHomework.Controls;

public partial class HomeworkPanel : UserControl
{
    private DateOnly _date;

    public event Action<DateOnly> DateChanged;
    
    public DateOnly Date
    {
        get => _date;
        set
        {
            _date = value;
            DateChanged?.Invoke(_date);
        }
    }
    
    public HomeworkPanel()
    {
        InitializeComponent();
        Date = DateOnly.FromDateTime(DateTime.Now);
    }

    public void Refresh(DateOnly date)
    {
        
    }
}