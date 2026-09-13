using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface ICareGroomingLogService
{
    Task<List<CareGroomingLog>> GetLogsForUserAsync(string userId);
    Task<CareGroomingLog?> GetDetailsAsync(int logId);

    Task<CareGroomingLogCreateData> GetCreateDataAsync(string userId, int petId);
    Task<ServiceResult<CareGroomingLog>> CreateAsync(CareGroomingLog log, string currentUserId);

    Task<CareGroomingLog?> GetEditDataAsync(int logId);
    Task<ServiceResult<CareGroomingLog>> UpdateAsync(int logId, CareGroomingLog log, string currentUserId);

    Task<CareGroomingLog?> GetDeleteDataAsync(int logId);
    Task<ServiceResult<bool>> DeleteAsync(int logId, string currentUserId);
    Task<List<CareGroomingLog>> GetCaresByPetIdAsync(int petId = 0,string? userId = null);
}

public class CareGroomingLogCreateData
{
    public bool ShouldRedirectToPets { get; set; }
    public string? ErrorMessage { get; set; }
    public CareGroomingLog? Log { get; set; }
}