namespace AntonReleaseCenter.Core.Models;

public record Admin(
    Guid AdminId,
    string Username,
    string PasswordHash
);