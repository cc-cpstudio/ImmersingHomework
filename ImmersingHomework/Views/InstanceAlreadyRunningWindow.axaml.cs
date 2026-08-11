using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ImmersingHomework.Views;

public partial class InstanceAlreadyRunningWindow : Window
{
    public InstanceAlreadyRunningWindow()
    {
        InitializeComponent();
    }

    private void ConfirmButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
