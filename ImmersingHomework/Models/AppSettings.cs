using System.Collections.ObjectModel;
using Avalonia.Media;

namespace ImmersingHomework.Models;

public class AppSettings
{
    public static AppSettings Instance { get; } = new AppSettings();

    public ObservableCollection<string> Subjects { get; set; } = [];
    public ObservableCollection<TagModel> Tags { get; set; } = [];
    
    public bool FirstLaunch { get; set; } = true;
    
    public ObservableProperty<bool> LaunchAtStartup { get; set; } = new(false);
    
    public ObservableProperty<bool> EnableClassIslandIPCService { get; set; } = new(false);
}