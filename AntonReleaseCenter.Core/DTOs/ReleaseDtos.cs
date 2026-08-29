using AntonReleaseCenter.Core.Models;

namespace AntonReleaseCenter.Core.DTOs;

public record CreateReleaseRequest(
    Guid SoftwareId,
    Guid ChannelId,
    PlatformEnum Platform,
    Version Version,
    string UpdateLog,
    string FilePath,
    int FileSize,
    string FileHash,
    bool IsForceUpdate,
    bool IsOnline
);

public record UpdateReleaseRequest(
    Guid ChannelId,
    PlatformEnum Platform,
    Version Version,
    string UpdateLog,
    string FilePath,
    int FileSize,
    string FileHash,
    bool IsForceUpdate,
    bool IsOnline
);
