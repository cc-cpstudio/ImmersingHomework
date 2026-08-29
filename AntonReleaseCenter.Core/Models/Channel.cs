namespace AntonReleaseCenter.Core.Models;

public record Channel(
    Guid ChannelId,
    Guid SoftwareId,
    int ChannelCode,
    string ChannelName,
    int GrayScalePercent
);