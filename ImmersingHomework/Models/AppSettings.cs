using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using ImmersingHomework.Services;
using Serilog;

namespace ImmersingHomework.Models;

public class AppSettings
{
    public static AppSettings Instance { get; } = new AppSettings();

    private readonly ILogger _logger = Log.ForContext<AppSettings>();
    private AppSettingsStorageService _storageService = new();
    private bool _isDirty;

    public ObservableCollection<string> Subjects { get; set; } = [];
    public ObservableCollection<TagModel> Tags { get; set; } = [];
    
    public bool FirstLaunch { get; set; } = true;
    
    public ObservableProperty<bool> LaunchAtStartup { get; set; } = new(false);
    
    public ObservableProperty<bool> EnableClassIslandIPCService { get; set; } = new(false);

    public AppSettings()
    {
    }

    public void Initialize()
    {
        _logger.Information("开始加载应用设置");
        var loaded = _storageService.Load();
        Subjects.Clear();
        foreach (var subject in loaded.Subjects)
        {
            Subjects.Add(subject);
        }
        Tags.Clear();
        foreach (var tag in loaded.Tags)
        {
            Tags.Add(tag);
        }
        FirstLaunch = loaded.FirstLaunch;
        LaunchAtStartup.Value = loaded.LaunchAtStartup.Value;
        EnableClassIslandIPCService.Value = loaded.EnableClassIslandIPCService.Value;
        
        SubscribeToChanges();
        _logger.Information("应用设置加载完成");
    }

    private void SubscribeToChanges()
    {
        Subjects.CollectionChanged += (s, e) => MarkDirty();
        Tags.CollectionChanged += (s, e) => MarkDirty();
        
        LaunchAtStartup.ValueChanged += _ => MarkDirty();
        EnableClassIslandIPCService.ValueChanged += _ => MarkDirty();
    }

    private void MarkDirty()
    {
        if (!_isDirty)
        {
            _isDirty = true;
            System.Threading.Tasks.Task.Delay(300).ContinueWith(_ => Save());
        }
    }

    public void Save()
    {
        _isDirty = false;
        _storageService.Save(this);
    }
}