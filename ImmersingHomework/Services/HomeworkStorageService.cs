using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ImmersingHomework.Models;

namespace ImmersingHomework.Services;

public class HomeworkStorageService
{
    private string GetFilePath(DateOnly date)
    {
        return Path.Combine(
            Directory.GetCurrentDirectory(), 
            "Data", 
            "Homeworks", 
            $"{date.Year:D4}-{date.Month:D2}-{date.Day:D2}.json"
        );
    }

    private string GetDataDir()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "Data", "Homeworks");
    }

    // 检查指定日期的作业文件是否存在
    public bool Exists(DateOnly date)
    {
        return File.Exists(GetFilePath(date));
    }

    public void Save(Homework homework)
    {
        var dataDir = GetDataDir();
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }
        string json = JsonSerializer.Serialize(homework, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(GetFilePath(homework.Date), json);
    }

    public async Task SaveAsync(Homework homework)
    {
        var dataDir = GetDataDir();
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }
        string json = JsonSerializer.Serialize(homework, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(GetFilePath(homework.Date), json);
    }

    public Homework? Load(DateOnly date)
    {
        try
        {
            var filePath = GetFilePath(date);
            if (!File.Exists(filePath))
            {
                return new Homework(date, []);
            }
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Homework>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception)
        {
            return new Homework(date, []);
        }
    }
}