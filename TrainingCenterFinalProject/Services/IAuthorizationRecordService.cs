using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IAuthorizationRecordService
{
    Task<List<AuthorizationRecord>> GetUserRecordsAsync(string userId);
    Task<bool> ChangeStatusAsync(string dataAuthorizationNumber, bool isAuthorized);
    Task<AuthorizationRecord?> GetByIdAsync(string dataAuthorizationNumber);

    Task<AuthorizationRecordCreateData?> GetCreateDataAsync(int petId, string institutionId);
    Task<string?> GetInstitutionIdByOwnerAsync(string ownerId);

    Task<ServiceResult<AuthorizationRecord>> CreateAsync(AuthorizationRecord authorizationRecord);
    Task<ServiceResult<AuthorizationRecord>> CreatePendingAsync(AuthorizationRecord authorizationRecord);

    Task<ServiceResult<AuthorizationRecord>> UpdateAsync(string dataAuthorizationNumber, AuthorizationRecord authorizationRecord);
    Task<bool> DeleteAsync(string dataAuthorizationNumber);
}

public class AuthorizationRecordCreateData
{
    public AuthorizationRecord Record { get; set; } = new();
    public string? PetName { get; set; }
    public string? InstitutionName { get; set; }
}