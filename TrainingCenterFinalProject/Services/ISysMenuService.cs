using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface ISysMenuService
{
    Task<List<SysMenu>> GetAllAsync();
    Task<SysMenu?> GetByIdAsync(int menuId);

    Task<ServiceResult<SysMenu>> CreateAsync(SysMenu sysMenu);
    Task<ServiceResult<SysMenu>> UpdateAsync(int menuId, SysMenu sysMenu);
    Task<ServiceResult<bool>> DeleteAsync(int menuId);
}