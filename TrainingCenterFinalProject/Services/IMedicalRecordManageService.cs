using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IMedicalRecordManageService
{
    Task<MedicalRecordManageIndexData?> GetIndexDataAsync(string userId, int petId = 0);
    Task<List<MedicalRecord>?> GetDetailsByPetAsync(int petId, string institutionId);

    Task<MedicalRecordManageCreateData?> GetCreateDataAsync(string userId, int petId);
    Task<ServiceResult<MedicalRecord>> CreateAsync(MedicalRecord medicalRecord, IFormFile? uploadFile, string vetId);

    Task<ServiceResult<bool>> DeleteFileAsync(int recordId);
    Task<ServiceResult<bool>> UploadFileAsync(int recordId, IFormFile uploadFile);

    Task<MedicalRecord?> GetEditDataAsync(int recordId);
    Task<ServiceResult<MedicalRecord>> UpdateAsync(int recordId, MedicalRecord medicalRecord, IFormFile? uploadFile, string? existingFileName);

    Task<MedicalRecord?> GetDeleteDataAsync(int recordId);
    Task<ServiceResult<bool>> DeleteAsync(int recordId);

    ServiceResult<string> GetDownloadFilePath(string fileName);
}

public class MedicalRecordManageIndexData
{
    public string InstitutionID { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public List<MedicalRecord> Records { get; set; } = new();
}

public class MedicalRecordManageCreateData
{
    public int PetID { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string InstitutionID { get; set; } = string.Empty;
}