namespace AntonReleaseCenter.Core.DTOs;

public record CreateAdminRequest(
    string Username,
    string PasswordHash
);

public record UpdateAdminRequest(
    string Username,
    string PasswordHash
);

public record AdminResponse(
    Guid AdminId,
    string Username
);

public record ChangePasswordRequest(
    string OldPasswordHash,
    string NewPasswordHash
);
