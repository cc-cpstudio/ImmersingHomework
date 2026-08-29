namespace AntonReleaseCenter.Core.DTOs;

public record CheckUpdateResponse(
    bool HasUpdate,
    Version? LatestVersion,
    string? UpdateLog,
    string? DownloadUrl,
    int? FileSize,
    string? FileHash,
    bool IsForceUpdate
);
