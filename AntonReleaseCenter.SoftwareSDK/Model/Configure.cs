using AntonReleaseCenter.Core.Models;

namespace AntonReleaseCenter.SoftwareSDK.Model;

public record Configure
{
    public required string Url { get; init; }
    public required string SoftwareKey { get; init; }
    public required int ChannelCode { get; init; }
    public required PlatformEnum Platform { get; init; }
}