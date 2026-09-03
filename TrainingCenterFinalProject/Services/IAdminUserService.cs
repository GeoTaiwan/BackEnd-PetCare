using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.ViewModels;

namespace TrainingCenterFinalProject.Services;

public interface IAdminUserService
{
    Task<List<Role>> GetRolesAsync();
    Task<ServiceResult<string>> CreateUserAsync(AdminUserCreateViewModel model);
    Task<AdminResetPasswordViewModel?> GetResetPasswordModelAsync(string userId);
    Task<ServiceResult<bool>> ResetPasswordAsync(AdminResetPasswordViewModel model);
}
