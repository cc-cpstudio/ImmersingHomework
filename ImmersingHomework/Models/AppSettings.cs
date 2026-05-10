using System.Collections.ObjectModel;
using Avalonia.Media;

namespace ImmersingHomework.Models;

public class AppSettings
{
    public static AppSettings Instance { get; } = new AppSettings();

    public ObservableCollection<string> Subjects { get; set; } = [ "语文", "数学", "英语", "物理" ];
    
    public ObservableCollection<TagModel> Tags { get; set; } = [
        new TagModel() { Name = "1", Color = new(Color.FromRgb(0, 0, 0)) },
        new TagModel() { Name = "2", Color = new(Color.FromRgb(100, 100, 100)) },
        new TagModel() { Name = "3", Color = new(Color.FromRgb(200, 200, 200)) }
    ];
}