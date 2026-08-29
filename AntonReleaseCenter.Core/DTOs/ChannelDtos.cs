namespace AntonReleaseCenter.Core.DTOs;

public record CreateChannelRequest(
    int ChannelCode,
    string ChannelName,
    int GrayScalePercent
);

public record UpdateChannelRequest(
    string ChannelName,
    int GrayScalePercent
);
