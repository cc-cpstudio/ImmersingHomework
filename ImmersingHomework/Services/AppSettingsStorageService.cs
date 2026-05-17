using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ImmersingHomework.Models;

namespace ImmersingHomework.Services;

public class AppSettingsStorageService
{
    private string GetFilePath()
    {
        return Path.Combine(
            Directory.GetCurrentDirectory(), 
            "Data", 
            "Settings.json"
        );
    }

    private string GetDataDir()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "Data");
    }

    // 检查设置文件是否存在
    public bool Exists()
    {
        return File.Exists(GetFilePath());
    }

    public void Save(AppSettings settings)
    {
        var dataDir = GetDataDir();
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(GetFilePath(), json);
    }

    public async Task SaveAsync(AppSettings settings)
    {
        var dataDir = GetDataDir();
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(GetFilePath(), json);
    }

    public AppSettings Load()
    {
        try
        {
            var filePath = GetFilePath();
            if (!File.Exists(filePath))
            {
                return new AppSettings();
            }
            string json = File.ReadAllText(filePath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return settings ?? new AppSettings();
        }
        catch (Exception)
        {
            return new AppSettings();
        }
    }
}
