using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AntonReleaseCenter.Core.DTOs;
using AntonReleaseCenter.Core.Models;
using AntonReleaseCenter.SoftwareSDK.Model;
using AntonReleaseCenter.SoftwareSDK.Services;
using Serilog;
using SoftwareVersion = AntonReleaseCenter.Core.Models.Version;

namespace ImmersingHomework.Services;

public static class UpdateService
{
    private static readonly ILogger _logger = Log.ForContext(typeof(UpdateService));

    private const string ReleaseCenterUrl = "https://arc.ieducation.top";
    private const string SoftwareKey = "ImmersingHomework";
    private const int ChannelCode = 0;

    // 与 Launcher.exe 约定：更新下载完成后写入此标记文件，供 Launcher 在下次启动时应用更新
    private const string UpdateFlagFileName = "update.flag";

    public static SoftwareVersion GetCurrentVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return new SoftwareVersion(
            version?.Major ?? 0,
            version?.Minor ?? 0,
            version?.Build ?? 0,
            version?.Revision ?? 0);
    }

    public static PlatformEnum GetCurrentPlatform()
    {
        if (OperatingSystem.IsWindows())
            return Environment.Is64BitOperatingSystem ? PlatformEnum.Windows_x64 : PlatformEnum.Windows_x86;
        if (OperatingSystem.IsMacOS())
            return RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                ? PlatformEnum.MacOS_AppleSilicon
                : PlatformEnum.MacOS_Intel;
        if (OperatingSystem.IsLinux())
            return Environment.Is64BitOperatingSystem ? PlatformEnum.AppImage_x64 : PlatformEnum.AppImage_x86;
        return PlatformEnum.Windows_x64;
    }

    public static async Task<CheckUpdateResponse?> CheckUpdateAsync(CancellationToken ct = default)
    {
        var configure = new Configure
        {
            Url = ReleaseCenterUrl,
            SoftwareKey = SoftwareKey,
            ChannelCode = ChannelCode,
            Platform = GetCurrentPlatform()
        };

        var service = new UpdateCheckService(App.HttpClient, configure);
        var currentVersion = GetCurrentVersion();
        _logger.Information("开始检查更新，当前版本: {Version}，平台: {Platform}", currentVersion, configure.Platform);

        var result = await service.CheckUpdateAsync(currentVersion.ToString(), ct: ct);
        if (result is null)
        {
            _logger.Information("未获取到更新信息");
            return null;
        }

        _logger.Information("检查更新完成，是否有更新: {HasUpdate}，最新版本: {LatestVersion}，是否强制更新: {IsForceUpdate}",
            result.HasUpdate, result.LatestVersion, result.IsForceUpdate);
        return result;
    }

    public static async Task<string?> DownloadUpdateAsync(
        CheckUpdateResponse update,
        IProgress<double>? progress = null,
        CancellationToken ct = default)
    {
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        if (string.IsNullOrWhiteSpace(update.DownloadUrl))
        {
            _logger.Warning("更新下载地址为空，无法下载");
            return null;
        }

        var version = update.LatestVersion?.ToString() ?? "unknown";
        var targetDir = GetUpdateDirectory(version);
        if (!Directory.Exists(targetDir))
        {
            _logger.Information("创建更新下载目录: {TargetDir}", targetDir);
            Directory.CreateDirectory(targetDir);
        }

        var fileName = GetFileNameFromUrl(update.DownloadUrl, version);
        var fileLocation = Path.Combine(targetDir, fileName);

        var release = new SoftwareRelease(
            SoftwareReleaseId: Guid.Empty,
            SoftwareId: Guid.Empty,
            ChannelId: Guid.Empty,
            Platform: GetCurrentPlatform(),
            Version: update.LatestVersion ?? new SoftwareVersion(0, 0, 0, 0),
            UpdateLog: update.UpdateLog ?? string.Empty,
            FilePath: update.DownloadUrl,
            FileSize: update.FileSize ?? 0,
            FileHash: update.FileHash ?? string.Empty,
            IsForceUpdate: update.IsForceUpdate,
            ReleaseTime: DateTime.MinValue,
            IsOnline: true);

        _logger.Information("开始下载更新，目标路径: {FileLocation}", fileLocation);
        var downloadService = new FileDownloadService();
        await downloadService.DownloadRelease(release, fileLocation, progress ?? new Progress<double>(), ct);
        _logger.Information("更新下载完成: {FileLocation}", fileLocation);

        WriteUpdateFlag();
        return fileLocation;
    }

    public static string GetUpdateDirectory(string version)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "Temp", $"Update_v{version}");
    }

    // 在 Launcher.exe 同目录下写入更新标记文件，Launcher 下次启动时会应用更新
    public static void WriteUpdateFlag()
    {
        var flagPath = GetUpdateFlagPath();
        try
        {
            File.WriteAllText(flagPath, DateTime.Now.ToString("O"));
            _logger.Information("已写入更新标记文件: {FlagPath}", flagPath);
        }
        catch (Exception e)
        {
            _logger.Warning(e, "写入更新标记文件失败: {FlagPath}", flagPath);
        }
    }

    public static string GetUpdateFlagPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), UpdateFlagFileName);
    }

    private static string GetFileNameFromUrl(string url, string version)
    {
        try
        {
            var uri = new Uri(url);
            var name = Path.GetFileName(uri.AbsolutePath);
            if (!string.IsNullOrWhiteSpace(name))
                return name;
        }
        catch (Exception e)
        {
            _logger.Warning(e, "解析下载地址文件名失败: {Url}", url);
        }

        return $"ImmersingHomework_{version}.zip";
    }
}
