using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImmersingHomework.Models;
using Serilog;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using PointF = SixLabors.ImageSharp.PointF;
using RectangleF = SixLabors.ImageSharp.RectangleF;

namespace ImmersingHomework.Services;

public class HomeworkImageService
{
    private static readonly ILogger _logger = Log.ForContext<HomeworkImageService>();
    
    private const int ImageWidth = 384;
    private const int Margin = 12;
    private const int Spacing = 12;
    private const int TagHeight = 36;
    private const int TagMinWidth = 70;
    private const float TagPadding = 20;
    
    private static FontCollection? _fontCollection;
    private static Font? _fontBold;
    private static Font? _fontMedium;
    private static Font? _fontRegular;

    public static void InitializeFonts()
    {
        try
        {
            _fontCollection = new FontCollection();
            var fontPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Fonts");
            
            if (Directory.Exists(fontPath))
            {
                _fontCollection.Add(Path.Combine(fontPath, "HarmonyOS_SansSC_Bold.ttf"));
                _fontCollection.Add(Path.Combine(fontPath, "HarmonyOS_SansSC_Medium.ttf"));
                _fontCollection.Add(Path.Combine(fontPath, "HarmonyOS_SansSC_Regular.ttf"));
            }
            
            var families = _fontCollection.Families.ToList();
            if (families.Count > 0)
            {
                _fontBold = families[0].CreateFont(36, FontStyle.Bold);
                _fontMedium = families[0].CreateFont(24, FontStyle.Bold);
                _fontRegular = families[0].CreateFont(20, FontStyle.Regular);
            }
            else
            {
                _fontBold = SystemFonts.CreateFont("Microsoft YaHei", 36, FontStyle.Bold);
                _fontMedium = SystemFonts.CreateFont("Microsoft YaHei", 24, FontStyle.Bold);
                _fontRegular = SystemFonts.CreateFont("Microsoft YaHei", 20, FontStyle.Regular);
            }
            
            _logger.Information("字体初始化完成");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "字体初始化失败，使用系统默认字体");
            _fontBold = SystemFonts.CreateFont("Microsoft YaHei", 36, FontStyle.Bold);
            _fontMedium = SystemFonts.CreateFont("Microsoft YaHei", 24, FontStyle.Bold);
            _fontRegular = SystemFonts.CreateFont("Microsoft YaHei", 20, FontStyle.Regular);
        }
    }

    public static void HomeworkToImage(Homework homework, string outputPath)
    {
        try
        {
            if (_fontCollection == null || _fontBold == null || _fontMedium == null || _fontRegular == null)
            {
                InitializeFonts();
            }

            _logger.Information("开始生成作业图片，日期: {Date}", homework.Date);

            var elements = CalculateElements(homework);
            var totalHeight = CalculateTotalHeight(elements);

            using var image = new Image<Rgba32>(ImageWidth, totalHeight);
            image.Mutate(x => x.Fill(Color.White));

            var currentY = Margin;
            
            foreach (var element in elements)
            {
                DrawElement(image, element, ref currentY);
            }

            image.SaveAsPng(outputPath);
            _logger.Information("作业图片已保存到: {Path}", outputPath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "生成作业图片失败");
            throw;
        }
    }

    private static List<object> CalculateElements(Homework homework)
    {
        var elements = new List<object>();
        
        elements.Add(new TextElement
        {
            Text = $"{homework.Date.Month}月{homework.Date.Day}日作业",
            Font = _fontBold!,
            LineHeight = 44,
            IsLast = false
        });

        foreach (var subject in homework.HomeworkItems.GroupBy(x => x.Subject))
        {
            elements.Add(new TextElement
            {
                Text = $"科目：{subject.Key}",
                Font = _fontMedium!,
                LineHeight = 29,
                IsLast = false
            });

            foreach (var item in subject)
            {
                elements.Add(new TextElement
                {
                    Text = item.Content,
                    Font = _fontRegular!,
                    LineHeight = 24,
                    IsLast = false
                });

                if (item.Tags != null && item.Tags.Count > 0)
                {
                    var tagModels = new List<TagElement>();
                    foreach (var tagName in item.Tags)
                    {
                        var tagModel = AppSettings.Instance.Tags.FirstOrDefault(t => t.Name == tagName);
                        if (tagModel != null)
                        {
                            tagModels.Add(new TagElement
                            {
                                Name = tagName,
                                Color = Color.FromRgb(tagModel.Color.Color.R, tagModel.Color.Color.G, tagModel.Color.Color.B)
                            });
                        }
                        else
                        {
                            tagModels.Add(new TagElement
                            {
                                Name = tagName,
                                Color = Color.FromRgb(220, 240, 255)
                            });
                        }
                    }

                    elements.Add(new TagGroupElement { Tags = tagModels });
                }
            }
        }

        elements.Add(new TextElement
        {
            Text = "Generated by ImmersingHomework",
            Font = _fontRegular!,
            LineHeight = 19,
            IsLast = true,
            FontSize = 16,
            Color = Color.FromRgb(166, 166, 166)
        });

        return elements;
    }

    private static int CalculateTotalHeight(List<object> elements)
    {
        var height = Margin * 2;
        var hasPrevious = false;

        foreach (var element in elements)
        {
            if (hasPrevious)
            {
                height += Spacing;
            }

            switch (element)
            {
                case TextElement text:
                    height += text.LineHeight;
                    break;
                case TagGroupElement tagGroup:
                    height += CalculateTagGroupHeight(tagGroup);
                    break;
            }

            hasPrevious = true;
        }

        return height;
    }

    private static int CalculateTagGroupHeight(TagGroupElement tagGroup)
    {
        float currentX = Margin;
        var rows = 1;

        foreach (var tag in tagGroup.Tags)
        {
            var tagWidth = CalculateTagWidth(tag);
            if (currentX + tagWidth > ImageWidth - Margin)
            {
                rows++;
                currentX = Margin;
            }
            currentX += tagWidth + Spacing;
        }

        return rows * TagHeight + (rows - 1) * Spacing;
    }

    private static float CalculateTagWidth(TagElement tag)
    {
        var textSize = TextMeasurer.MeasureBounds(tag.Name, new TextOptions(_fontRegular!));
        var width = TagPadding * 2 + textSize.Width;
        return Math.Max(width, TagMinWidth);
    }

    private static void DrawElement(Image<Rgba32> image, object element, ref int currentY)
    {
        switch (element)
        {
            case TextElement text:
                DrawText(image, text, ref currentY);
                break;
            case TagGroupElement tagGroup:
                DrawTagGroup(image, tagGroup, ref currentY);
                break;
        }
    }

    private static void DrawText(Image<Rgba32> image, TextElement textElement, ref int currentY)
    {
        var font = textElement.FontSize.HasValue ? 
            textElement.Font.Family.CreateFont(textElement.FontSize.Value) : 
            textElement.Font;
        
        var textSize = TextMeasurer.MeasureBounds(textElement.Text, new TextOptions(font));

        float x;
        if (textElement.IsLast)
        {
            x = (ImageWidth - textSize.Width) / 2;
        }
        else
        {
            x = Margin;
        }

        var y = currentY;
        
        image.Mutate(ctx => ctx.DrawText(
            textElement.Text,
            font,
            textElement.Color ?? Color.Black,
            new PointF(x, y)));

        currentY += textElement.LineHeight;
    }

    private static void DrawTagGroup(Image<Rgba32> image, TagGroupElement tagGroup, ref int currentY)
    {
        float currentX = Margin;
        var startY = currentY;

        foreach (var tag in tagGroup.Tags)
        {
            var tagWidth = CalculateTagWidth(tag);
            if (currentX + tagWidth > ImageWidth - Margin)
            {
                currentX = Margin;
                currentY += TagHeight + Spacing;
            }

            DrawTag(image, tag, currentX, currentY, tagWidth);
            currentX += tagWidth + Spacing;
        }

        var tagGroupHeight = CalculateTagGroupHeight(tagGroup);
        currentY = startY + tagGroupHeight;
    }

    private static void DrawTag(Image<Rgba32> image, TagElement tag, float x, float y, float width)
    {
        var rect = new RectangleF(x, y, width, TagHeight);
        
        image.Mutate(ctx => ctx.Fill(tag.Color, rect));
        
        var font = _fontRegular!.Family.CreateFont(14);
        var textSize = TextMeasurer.MeasureBounds(tag.Name, new TextOptions(font));
        var textX = x + (width - textSize.Width) / 2;
        var textY = y + (TagHeight - textSize.Height) / 2;
        
        image.Mutate(ctx => ctx.DrawText(tag.Name, font, Color.Black, new PointF(textX, textY)));
    }
}

internal class TextElement
{
    public string Text { get; set; } = string.Empty;
    public Font Font { get; set; } = null!;
    public int LineHeight { get; set; }
    public bool IsLast { get; set; }
    public float? FontSize { get; set; }
    public Color? Color { get; set; }
}

internal class TagElement
{
    public string Name { get; set; } = string.Empty;
    public Color Color { get; set; }
}

internal class TagGroupElement
{
    public List<TagElement> Tags { get; set; } = [];
}