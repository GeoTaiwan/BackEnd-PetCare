using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IUserProfileService
{
    Task<List<UserProfile>> GetAllAsync();
    Task<UserProfile?> GetByIdAsync(string userId);

    Task<ServiceResult<UserProfile>> CreateAsync(UserProfile userProfile);
    Task<ServiceResult<UserProfile>> UpdateAsync(string userId, UserProfile userProfile);
    Task<ServiceResult<bool>> DeleteAsync(string userId);
}