namespace AntonReleaseCenter.Core.DTOs;

public record CreateSoftwareRequest(
    string AppKey,
    string Name,
    string Description,
    bool IsEnabled
);

public record UpdateSoftwareRequest(
    string Name,
    string Description,
    bool IsEnabled
);
