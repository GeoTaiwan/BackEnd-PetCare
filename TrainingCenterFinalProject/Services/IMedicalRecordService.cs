using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IMedicalRecordService
{
    Task<MedicalRecordIndexData> GetIndexDataAsync(string userId, int petId = 0);
    Task<MedicalRecord?> GetDetailsAsync(int recordId);

    Task<MedicalRecordCreateData> GetCreateDataAsync(string userId, int petId = 0);
    Task<ServiceResult<MedicalRecord>> CreateAsync(MedicalRecord medicalRecord, IFormFile? uploadFile);

    Task<ServiceResult<bool>> DeleteFileAsync(int recordId);
    Task<ServiceResult<bool>> UploadFileAsync(int recordId, IFormFile uploadFile);

    Task<MedicalRecord?> GetEditDataAsync(int recordId);
    Task<ServiceResult<MedicalRecord>> UpdateAsync(int recordId, MedicalRecord medicalRecord, IFormFile? uploadFile, string? existingFileName);

    Task<MedicalRecord?> GetDeleteDataAsync(int recordId);
    Task<ServiceResult<bool>> DeleteAsync(int recordId);

    ServiceResult<string> GetDownloadFilePath(string fileName);
}

public class MedicalRecordIndexData
{
    public List<MedicalRecord> Records { get; set; } = new();
    public List<Pet> Pets { get; set; } = new();
    public bool NoPet { get; set; }
    public int SelectedPetId { get; set; }
}

public class MedicalRecordCreateData
{
    public List<Pet> Pets { get; set; } = new();
    public int SelectedPetId { get; set; }
    public bool NoPet { get; set; }
}