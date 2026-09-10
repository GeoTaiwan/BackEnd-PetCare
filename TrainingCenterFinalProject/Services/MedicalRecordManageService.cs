using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class MedicalRecordManageService : IMedicalRecordManageService
{
    private readonly PetCareDbContext _context;
    private readonly IWebHostEnvironment _env;

    public MedicalRecordManageService(PetCareDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<MedicalRecordManageIndexData?> GetIndexDataAsync(string userId, int petId = 0)
    {
        var institutionId = await _context.Employments
            .Where(v => v.UserID == userId)
            .Select(v => v.InstitutionID)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(institutionId)) return null;

        var institutionName = await _context.Institutions
            .Where(i => i.InstitutionID == institutionId)
            .Select(i => i.Name)
            .FirstOrDefaultAsync() ?? string.Empty;

        var query = _context.MedicalRecords
            .Include(m => m.Pet)
                .ThenInclude(p => p.AuthorizationRecords.Where(ar => ar.InstitutionID == institutionId))
            .Include(m => m.Vet)
                .ThenInclude(v => v.Employments.Where(e => e.InstitutionID == institutionId))
            .Include(m => m.RecordDiagnoses)
                .ThenInclude(rd => rd.MedicalVocabularyDictionary)
            .Include(m => m.Prescriptions)
            .AsQueryable();

        if (petId > 0)
        {
            query = query.Where(m => m.PetID == petId);
        }

        query = query.Where(m => m.Pet!.AuthorizationRecords.Any(ar =>
            ar.InstitutionID == institutionId &&
            ar.Status == "Authorized" &&
            ar.GrantedDate < DateTime.Now &&
            ar.RevokedDate >= DateTime.Now));

        var records = await query.OrderByDescending(m => m.VisitDate).ToListAsync();

        return new MedicalRecordManageIndexData
        {
            InstitutionID = institutionId,
            InstitutionName = institutionName,
            Records = records
        };
    }

    public async Task<List<MedicalRecord>?> GetDetailsByPetAsync(int petId, string institutionId)
    {
        var vetId = await _context.Employments
            .Where(v => v.InstitutionID == institutionId)
            .Select(v => v.UserID)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(vetId)) return null;

        return await _context.MedicalRecords
            .Include(m => m.Pet)
            .Include(m => m.Vet)
            .Include(m => m.RecordDiagnoses).ThenInclude(rd => rd.MedicalVocabularyDictionary)
            .Include(m => m.Prescriptions)
            .Where(m => m.PetID == petId && m.VetID == vetId)
            .ToListAsync();
    }

    public async Task<MedicalRecordManageCreateData?> GetCreateDataAsync(string userId, int petId)
    {
        if (petId <= 0) return null;

        var petName = await _context.Pets
            .Where(p => p.PetID == petId)
            .Select(p => p.Name)
            .FirstOrDefaultAsync();

        var institutionId = await _context.Employments
            .Where(v => v.UserID == userId)
            .Select(v => v.InstitutionID)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(petName) || string.IsNullOrEmpty(institutionId)) return null;

        return new MedicalRecordManageCreateData
        {
            PetID = petId,
            PetName = petName,
            InstitutionID = institutionId
        };
    }

    public async Task<ServiceResult<MedicalRecord>> CreateAsync(MedicalRecord medicalRecord, IFormFile? uploadFile, string vetId)
    {
        var result = new ServiceResult<MedicalRecord> { IsSuccess = false };

        medicalRecord.VetID = vetId;

        if (uploadFile != null && uploadFile.Length > 0)
        {
            medicalRecord.TestResultsFile = await SaveUploadFileAsync(uploadFile);
        }

        _context.MedicalRecords.Add(medicalRecord);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = medicalRecord;
        return result;
    }

    public async Task<ServiceResult<bool>> DeleteFileAsync(int recordId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var medicalRecord = await _context.MedicalRecords.FirstOrDefaultAsync(m => m.RecordID == recordId);
        if (medicalRecord == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        if (!string.IsNullOrEmpty(medicalRecord.TestResultsFile))
        {
            var filePath = Path.Combine(_env.WebRootPath, "uploads", "mrFiles", medicalRecord.TestResultsFile);
            if (File.Exists(filePath)) File.Delete(filePath);

            medicalRecord.TestResultsFile = null;
            await _context.SaveChangesAsync();
        }

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }

    public async Task<ServiceResult<bool>> UploadFileAsync(int recordId, IFormFile uploadFile)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        if (uploadFile == null || uploadFile.Length == 0)
        {
            result.ErrorMessage = "NO_FILE";
            return result;
        }

        var medicalRecord = await _context.MedicalRecords.FindAsync(recordId);
        if (medicalRecord == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        medicalRecord.TestResultsFile = await SaveUploadFileAsync(uploadFile);
        _context.Update(medicalRecord);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }

    public async Task<MedicalRecord?> GetEditDataAsync(int recordId)
    {
        return await _context.MedicalRecords
            .Include(mr => mr.Pet)
            .Include(mr => mr.Vet)
            .FirstOrDefaultAsync(mr => mr.RecordID == recordId);
    }

    public async Task<ServiceResult<MedicalRecord>> UpdateAsync(int recordId, MedicalRecord medicalRecord, IFormFile? uploadFile, string? existingFileName)
    {
        var result = new ServiceResult<MedicalRecord> { IsSuccess = false };

        if (recordId != medicalRecord.RecordID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        if (uploadFile != null && uploadFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(existingFileName))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, "uploads", "mrFiles", existingFileName);
                if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
            }

            medicalRecord.TestResultsFile = await SaveUploadFileAsync(uploadFile);
        }

        try
        {
            _context.Update(medicalRecord);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = medicalRecord;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.MedicalRecords.AnyAsync(e => e.RecordID == medicalRecord.RecordID);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<MedicalRecord?> GetDeleteDataAsync(int recordId)
    {
        return await _context.MedicalRecords.FirstOrDefaultAsync(m => m.RecordID == recordId);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int recordId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var medicalRecord = await _context.MedicalRecords.FindAsync(recordId);
        if (medicalRecord == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        if (!string.IsNullOrEmpty(medicalRecord.TestResultsFile))
        {
            var filePath = Path.Combine(_env.WebRootPath, "uploads", "mrFiles", medicalRecord.TestResultsFile);
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        _context.MedicalRecords.Remove(medicalRecord);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }

    public ServiceResult<string> GetDownloadFilePath(string fileName)
    {
        var result = new ServiceResult<string> { IsSuccess = false };

        if (string.IsNullOrEmpty(fileName))
        {
            result.ErrorMessage = "EMPTY_FILE_NAME";
            return result;
        }

        var filePath = Path.Combine(_env.WebRootPath, "uploads", "mrFiles", fileName);
        if (!File.Exists(filePath))
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        result.IsSuccess = true;
        result.Data = filePath;
        return result;
    }

    private async Task<string> SaveUploadFileAsync(IFormFile uploadFile)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "mrFiles");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(uploadFile.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await uploadFile.CopyToAsync(fileStream);

        return uniqueFileName;
    }
}