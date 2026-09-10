using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IInstitutionService
{
    Task<List<Institution>> GetMyInstitutionsAsync(string ownerId);
    Task<List<Institution>> GetInstitutionsForReservationAsync(string? typeId);

    Task<Institution?> GetInstitutionManageAsync(string institutionId);
    Task<Institution?> GetDetailsAsync(string institutionId);

    Task<InstitutionCreateCheckResult> CheckCreateEligibilityAsync(string userId);
    Task<ServiceResult<Institution>> CreateAsync(Institution institution, string currentUserId);

    Task<Institution?> GetByIdAsync(string institutionId);
    Task<ServiceResult<Institution>> UpdateAsync(Institution institution, bool fromDetails, string currentUserId);

    Task<ServiceResult<bool>> DeleteAsync(string institutionId);

    Task<List<SelectListItem>> GetInstitutionTypeSelectListAsync();
    Task<List<SelectListItem>> GetInstitutionStatusSelectListAsync();
}

public class InstitutionCreateCheckResult
{
    public bool HasPendingApplication { get; set; }
    public string? PendingInstitutionId { get; set; }
}