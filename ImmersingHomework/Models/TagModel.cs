using Avalonia.Media;
using Serilog;

namespace ImmersingHomework.Models;

public class TagModel
{
    private readonly ILogger _logger = Log.ForContext<TagModel>();
    public string Name { get; set; }
    public SolidColorBrush Color { get; set; }
    
    public TagModel()
    {
        _logger.Debug("TagModel 初始化");
    }
}