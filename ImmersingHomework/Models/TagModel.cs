using System;
using System.Text.Json.Serialization;
using Avalonia.Media;
using Serilog;

namespace ImmersingHomework.Models;

public class TagModel
{
    private readonly ILogger _logger = Log.ForContext<TagModel>();
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public SolidColorBrush Color { get; set; }

    [JsonPropertyName("Color")]
    public string ColorHex
    {
        get
        {
            if (Color == null)
                return "#00000000";
            var color = Color.Color;
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        set
        {
            try
            {
                if (value.StartsWith("#"))
                    value = value.Substring(1);
                if (value.Length == 6) // RGB 没有 A
                    value = "FF" + value;
                var a = Convert.ToByte(value.Substring(0, 2), 16);
                var r = Convert.ToByte(value.Substring(2, 2), 16);
                var g = Convert.ToByte(value.Substring(4, 2), 16);
                var b = Convert.ToByte(value.Substring(6, 2), 16);
                Color = new SolidColorBrush(Avalonia.Media.Color.FromArgb(a, r, g, b));
            }
            catch
            {
                _logger.Warning("无法解析颜色值: {Value}", value);
                Color = new SolidColorBrush(Colors.LightBlue);
            }
        }
    }
    
    public TagModel()
    {
        _logger.Debug("TagModel 初始化");
        Color = new SolidColorBrush(Colors.LightBlue);
    }
}