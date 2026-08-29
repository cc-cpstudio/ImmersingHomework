namespace AntonReleaseCenter.Core.Services;

public interface IAdminService
{
    Task<List<AdminResponse>> GetAllAdminsAsync();
    Task<AdminResponse?> GetAdminByIdAsync(Guid id);
    Task<AdminResponse> CreateAdminAsync(CreateAdminRequest request);
    Task<AdminResponse?> UpdateAdminAsync(Guid id, UpdateAdminRequest request);
    Task<bool> DeleteAdminAsync(Guid id);
    Task<bool> ChangePasswordAsync(Guid adminId, string oldPasswordHash, string newPasswordHash);
}
