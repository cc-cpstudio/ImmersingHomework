namespace AntonReleaseCenter.Core.Services;

public interface IReleaseService
{
    // Software
    Task<List<Software>> GetAllSoftwareAsync();
    Task<Software?> GetSoftwareByIdAsync(Guid id);
    Task<Software> CreateSoftwareAsync(CreateSoftwareRequest request);
    Task<Software?> UpdateSoftwareAsync(Guid id, UpdateSoftwareRequest request);
    Task<bool> DeleteSoftwareAsync(Guid id);

    // Channel
    Task<List<Channel>> GetChannelsBySoftwareIdAsync(Guid softwareId);
    Task<Channel?> GetChannelByIdAsync(Guid id);
    Task<Channel> CreateChannelAsync(Guid softwareId, CreateChannelRequest request);
    Task<Channel?> UpdateChannelAsync(Guid id, UpdateChannelRequest request);
    Task<bool> DeleteChannelAsync(Guid id);

    // Release
    Task<List<SoftwareRelease>> GetReleasesBySoftwareIdAsync(Guid softwareId, Guid? channelId);
    Task<List<SoftwareRelease>> GetReleasesBySoftwareNameAndPlatformAsync(string softwareName, PlatformEnum platform);
    Task<SoftwareRelease?> GetReleaseByIdAsync(Guid id);
    Task<SoftwareRelease> CreateReleaseAsync(CreateReleaseRequest request);
    Task<SoftwareRelease?> UpdateReleaseAsync(Guid id, UpdateReleaseRequest request);
    Task<bool> DeleteReleaseAsync(Guid id);
    Task<SoftwareRelease?> ToggleReleaseOnlineAsync(Guid id);

    // Client check update
    Task<CheckUpdateResponse?> CheckUpdateAsync(string appKey, int channelCode, PlatformEnum platform, Version currentVersion, string? deviceId);
}
