using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using ImmersingHomework.Services;

namespace ImmersingHomework.Models;

public class AppSettings
{
    public static AppSettings Instance { get; } = new AppSettings();

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
        var loaded = _storageService.Load();
        Subjects = new ObservableCollection<string>(loaded.Subjects);
        Tags = new ObservableCollection<TagModel>(loaded.Tags);
        FirstLaunch = loaded.FirstLaunch;
        LaunchAtStartup = new ObservableProperty<bool>(loaded.LaunchAtStartup.Value);
        EnableClassIslandIPCService = new ObservableProperty<bool>(loaded.EnableClassIslandIPCService.Value);
        
        SubscribeToChanges();
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