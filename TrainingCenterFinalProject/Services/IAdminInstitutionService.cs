using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IAdminInstitutionService
{
    Task<List<Institution>> GetAllAsync();
    Task<Institution?> GetDetailsAsync(string id);
    Task<Institution?> GetByIdAsync(string id);
    Task<Institution?> GetPendingInstitutionByOwnerAsync(string ownerId);

    Task<ServiceResult<Institution>> CreateInstitutionAsync(Institution institution, string currentUserId);
    Task<ServiceResult<Institution>> UpdateInstitutionAsync(Institution institution, bool fromDetails, string currentUserId);
    Task<ServiceResult<bool>> DeleteInstitutionAsync(string id);

    Task<List<InstitutionType>> GetInstitutionTypesAsync();
    Task<List<InstitutionStatus>> GetInstitutionStatusesAsync();
}