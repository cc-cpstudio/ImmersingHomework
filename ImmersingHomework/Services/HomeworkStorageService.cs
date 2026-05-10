using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ImmersingHomework.Models;

namespace ImmersingHomework.Services;

public class HomeworkStorageService
{
    public void Save(Homework homework)
    {
        string json = JsonSerializer.Serialize(homework, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Homeworks", $"{ homework.Date.ToString() }.json"), json);
    }

    public Homework? Load(DateOnly date)
    {
        try
        {
            string json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Homeworks",
                $"{date.ToString()}.json"));
            return JsonSerializer.Deserialize<Homework>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (FileNotFoundException)
        {
            return new Homework(date, []);
        }
    }
}