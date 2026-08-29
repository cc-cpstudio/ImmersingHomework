namespace AntonReleaseCenter.Core.Models;

public record SoftwareRelease(
    Guid SoftwareReleaseId,
    Guid SoftwareId,
    Guid ChannelId,
    PlatformEnum Platform,
    Version Version,
    string UpdateLog,
    string FilePath,
    int FileSize,
    string FileHash,
    bool IsForceUpdate,
    DateTime ReleaseTime,
    bool IsOnline
);