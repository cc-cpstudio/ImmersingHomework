using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using ImmersingHomework.Shared.Models;
using SkiaSharp;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace ImmersingHomework.Services;

public static class HomeworkQrCodeService
{
    public static string? GenerateQrCode(Homework homework, string outputPath)
    {
        string json = SerializeCompact(homework);
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = 300,
                Height = 300,
                Margin = 4,
                ErrorCorrection = ErrorCorrectionLevel.L
            }
        };

        ZXing.Rendering.PixelData pixelData;
        try
        {
            pixelData = qrCodeWriter.Write(json);
        }
        catch (WriterException)
        {
            return null;
        }

        using var bmp = new SKBitmap(pixelData.Width, pixelData.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        Marshal.Copy(pixelData.Pixels, 0, bmp.GetPixels(), pixelData.Pixels.Length);
        using var image = SKImage.FromBitmap(bmp);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        var qrOutputPath = Path.Combine(
            Path.GetDirectoryName(outputPath) ?? "",
            Path.GetFileNameWithoutExtension(outputPath) + "_qrc" + Path.GetExtension(outputPath));
        var fullPath = Path.GetFullPath(qrOutputPath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        using var stream = File.OpenWrite(fullPath);
        data.SaveTo(stream);
        return qrOutputPath;
    }

    public static Homework ParseQrCode(string qrCodeFilePath)
    {
        using var original = SKBitmap.Decode(qrCodeFilePath);
        using var bmp = new SKBitmap(original.Width, original.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        using var canvas = new SKCanvas(bmp);
        canvas.DrawBitmap(original, 0, 0);
        canvas.Flush();

        var reader = new BarcodeReaderGeneric { AutoRotate = true };
        reader.Options.TryInverted = true;
        var result = reader.Decode(bmp.Bytes, bmp.Width, bmp.Height, RGBLuminanceSource.BitmapFormat.BGRA32);
        if (result == null)
            throw new InvalidOperationException("无法从图片中解析QR码。");
        return DeserializeCompact(result.Text);
    }

    private static string SerializeCompact(Homework homework)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        writer.WriteStartObject();
        writer.WriteString("d", homework.Date.ToString("yyyy-MM-dd"));
        writer.WriteStartArray("i");
        foreach (var item in homework.HomeworkItems)
        {
            writer.WriteStartObject();
            writer.WriteString("id", item.Id);
            writer.WriteString("s", item.Subject);
            writer.WriteString("c", item.Content);
            if (item.Tags is { Count: > 0 })
            {
                writer.WriteStartArray("t");
                foreach (var tag in item.Tags)
                    writer.WriteStringValue(tag.Name);
                writer.WriteEndArray();
            }
            if (item.TemplateName is not null)
                writer.WriteString("n", item.TemplateName);
            if (item.TemplateParameters is { Count: > 0 })
            {
                writer.WriteStartArray("p");
                foreach (var param in item.TemplateParameters)
                    writer.WriteStringValue(param);
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static Homework DeserializeCompact(string json)
    {
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var date = DateOnly.Parse(root.GetProperty("d").GetString()!);
        var items = new List<HomeworkItem>();
        foreach (var elem in root.GetProperty("i").EnumerateArray())
        {
            var tags = new List<TagModel>();
            if (elem.TryGetProperty("t", out var t))
            {
                foreach (var tag in t.EnumerateArray())
                    tags.Add(new TagModel { Name = tag.GetString()! });
            }
            List<string>? templateParams = null;
            if (elem.TryGetProperty("p", out var p))
            {
                templateParams = new List<string>();
                foreach (var param in p.EnumerateArray())
                    templateParams.Add(param.GetString()!);
            }
            var item = new HomeworkItem(
                elem.GetProperty("s").GetString()!,
                elem.GetProperty("c").GetString()!,
                tags)
            {
                Id = elem.GetProperty("id").GetGuid(),
                TemplateName = elem.TryGetProperty("n", out var n) ? n.GetString() : null,
                TemplateParameters = templateParams
            };
            items.Add(item);
        }
        return new Homework(date, items);
    }
}