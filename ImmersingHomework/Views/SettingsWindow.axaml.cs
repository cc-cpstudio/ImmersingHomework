using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using ImmersingHomework.Views.SettingsPages;

namespace ImmersingHomework.Views;

public partial class SettingsWindow : FAAppWindow
{
    public SettingsWindow()
    {
        InitializeComponent();
        NavigationView.SelectedItem = NavigationView.MenuItems[0];
    }

    private void NavigationView_SelectionChanged(object? sender, FANavigationViewSelectionChangedEventArgs e)
    {
        if (NavigationView.SelectedItem is FANavigationViewItem item && item.Tag is string tag)
        {
            switch (tag)
            {
                case "Basic":
                    ContentFrame.Navigate(typeof(BasicSettingsPage));
                    break;
                case "Subject":
                    ContentFrame.Navigate(typeof(SubjectSettingsPage));
                    break;
                case "Tag":
                    ContentFrame.Navigate(typeof(TagSettingsPage));
                    break;
                case "About":
                    ContentFrame.Navigate(typeof(AboutPage));
                    break;
                case "Linkage":
                    ContentFrame.Navigate(typeof(LinkageSettingsPage));
                    break;
            }
        }
    }
}