using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using ImmersingHomework.Models;
using SkiaSharp;
using ZXing;
using ZXing.QrCode;

namespace ImmersingHomework.Services;

public static class HomeworkQrCodeService
{
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public static string GenerateQrCode(Homework homework, string outputPath)
    {
        string json = JsonSerializer.Serialize(homework, _options);
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = 300,
                Height = 300,
                Margin = 4
            }
        };
        var pixelData = qrCodeWriter.Write(json);
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
        return JsonSerializer.Deserialize<Homework>(result.Text, _options)!;
    }
}