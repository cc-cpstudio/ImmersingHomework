using AntonReleaseCenter.Core.Models;
using Downloader;

namespace AntonReleaseCenter.SoftwareSDK.Services;

public sealed class FileDownloadService
{
    public async Task DownloadRelease(SoftwareRelease release, string fileLocation, IProgress<double> progress, CancellationToken ct = default)
    {
        var opt = new DownloadConfiguration
        {
            ChunkCount = 8,
            RequestConfiguration = new() { UserAgent = "" }
        };
        var downloader = new DownloadBuilder()
            .WithUrl(release.FilePath)
            .WithFileLocation(fileLocation)
            .WithConfiguration(opt)
            .Build();
        downloader.DownloadProgressChanged += (s, e) => { progress.Report(e.ProgressPercentage); };
        await downloader.StartAsync(ct);
    }
}