using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.ViewModels;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IEmploymentService
{
    Task<string?> GetInstitutionIdByUserIdAsync(string userId);
    Task<string?> GetInstitutionNameAsync(string institutionId);
    Task<List<Employment>> GetEmploymentsByInstitutionAsync(string institutionId);

    Task<NewEmployeeViewModel?> BuildCreateViewModelAsync(string institutionId);
    Task<List<SelectListItem>> GetOccupationSelectListAsync();
    List<SelectListItem> GetStatusSelectList();

    Task<ServiceResult<Employment>> CreateAsync(NewEmployeeViewModel model);
    Task<Employment?> GetEmploymentForEditAsync(string currentUserId, string targetUserId);
    Task<ServiceResult<Employment>> UpdateAsync(string currentUserId, string targetUserId, Employment employment);

    Task<Employment?> GetByUserIdAsync(string userId);
    Task<ServiceResult<bool>> DeleteAsync(string userId);
}