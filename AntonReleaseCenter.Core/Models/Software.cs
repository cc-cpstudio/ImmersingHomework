namespace AntonReleaseCenter.Core.Models;

public record Software(
    Guid SoftwareId,
    string AppKey,
    string Name,
    string Description,
    bool IsEnabled
);