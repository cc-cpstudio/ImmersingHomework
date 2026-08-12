using System;
using System.Collections.Generic;
using System.Formats.Cbor;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ImmersingHomework.Shared.Models;
using SkiaSharp;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace ImmersingHomework.Services;

public static class HomeworkQrCodeService
{
    private const string DateKey = "d";
    private const string ItemsKey = "i";
    private const string IdKey = "id";
    private const string SubjectKey = "s";
    private const string ContentKey = "c";
    private const string TagsKey = "t";
    private const string TemplateNameKey = "n";
    private const string TemplateParamsKey = "p";

    public static string? GenerateQrCode(Homework homework, string outputPath)
    {
        byte[] payload = SerializeCompact(homework);
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
            pixelData = qrCodeWriter.Write(payload);
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

        byte[]? payload = result.RawBytes;
        if (payload == null)
        {
            // 回退：按 ISO-8859-1 从 Text 还原原始字节（与 ZXing 字节模式编码无损对应）
            if (string.IsNullOrEmpty(result.Text))
                throw new InvalidOperationException("QR码内容为空。");
            payload = Encoding.GetEncoding("ISO-8859-1").GetBytes(result.Text);
        }
        return DeserializeCompact(payload);
    }

    private static byte[] SerializeCompact(Homework homework)
    {
        var writer = new CborWriter(CborConformanceMode.Standard, convertIndefiniteLengthEncodings: false, useMultipleFrames: false);

        writer.WriteStartMap(2);
        writer.WriteString(DateKey);
        writer.WriteString(homework.Date.ToString("yyyy-MM-dd"));
        writer.WriteString(ItemsKey);
        writer.WriteStartArray(homework.HomeworkItems.Count);
        foreach (var item in homework.HomeworkItems)
        {
            int keyCount = 3; // id, s, c
            if (item.Tags is { Count: > 0 }) keyCount++;
            if (item.TemplateName is not null) keyCount++;
            if (item.TemplateParameters is { Count: > 0 }) keyCount++;

            writer.WriteStartMap(keyCount);

            writer.WriteString(IdKey);
            writer.WriteBytes(item.Id.ToByteArray());

            writer.WriteString(SubjectKey);
            writer.WriteString(item.Subject);

            writer.WriteString(ContentKey);
            writer.WriteString(item.Content);

            if (item.Tags is { Count: > 0 })
            {
                writer.WriteString(TagsKey);
                writer.WriteStartArray(item.Tags.Count);
                foreach (var tag in item.Tags)
                    writer.WriteString(tag.Name);
                writer.WriteEndArray();
            }

            if (item.TemplateName is not null)
            {
                writer.WriteString(TemplateNameKey);
                writer.WriteString(item.TemplateName);
            }

            if (item.TemplateParameters is { Count: > 0 })
            {
                writer.WriteString(TemplateParamsKey);
                writer.WriteStartArray(item.TemplateParameters.Count);
                foreach (var param in item.TemplateParameters)
                    writer.WriteString(param);
                writer.WriteEndArray();
            }

            writer.WriteEndMap();
        }
        writer.WriteEndArray();
        writer.WriteEndMap();

        return writer.Encode();
    }

    private static Homework DeserializeCompact(byte[] payload)
    {
        var reader = new CborReader(payload, CborConformanceMode.Standard);

        reader.ReadStartMap();
        ReadExpectedKey(reader, DateKey);
        var date = DateOnly.Parse(reader.ReadTextString());
        ReadExpectedKey(reader, ItemsKey);

        var items = new List<HomeworkItem>();
        int? arrayCount = reader.ReadStartArray();
        for (int i = 0; arrayCount.HasValue ? i < arrayCount.Value : reader.PeekState() != CborReaderState.EndArray; i++)
        {
            reader.ReadStartMap();
            string subject = string.Empty;
            string content = string.Empty;
            Guid id = Guid.Empty;
            var tags = new List<TagModel>();
            string? templateName = null;
            List<string>? templateParams = null;

            int? mapCount = reader.ReadStartMap();
            for (int k = 0; mapCount.HasValue ? k < mapCount.Value : reader.PeekState() != CborReaderState.EndMap; k++)
            {
                string key = reader.ReadTextString();
                switch (key)
                {
                    case IdKey:
                        id = new Guid(reader.ReadByteString());
                        break;
                    case SubjectKey:
                        subject = reader.ReadTextString();
                        break;
                    case ContentKey:
                        content = reader.ReadTextString();
                        break;
                    case TagsKey:
                        tags = ReadStringArray(reader);
                        break;
                    case TemplateNameKey:
                        templateName = reader.ReadTextString();
                        break;
                    case TemplateParamsKey:
                        templateParams = ReadStringArray(reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.ReadEndMap();

            items.Add(new HomeworkItem(subject, content, tags)
            {
                Id = id,
                TemplateName = templateName,
                TemplateParameters = templateParams
            });
        }
        reader.ReadEndArray();
        reader.ReadEndMap();

        return new Homework(date, items);
    }

    private static void ReadExpectedKey(CborReader reader, string expected)
    {
        string key = reader.ReadTextString();
        if (key != expected)
            throw new InvalidOperationException($"期望键 '{expected}'，实际 '{key}'。");
    }

    private static List<string> ReadStringArray(CborReader reader)
    {
        var list = new List<string>();
        int? count = reader.ReadStartArray();
        for (int i = 0; count.HasValue ? i < count.Value : reader.PeekState() != CborReaderState.EndArray; i++)
        {
            list.Add(reader.ReadTextString());
        }
        reader.ReadEndArray();
        return list;
    }
}
