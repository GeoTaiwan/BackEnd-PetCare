using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly PetCareDbContext _context;
    private readonly IWebHostEnvironment _env;
    private const string DefaultVetId = "U202607280002";

    public MedicalRecordService(PetCareDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<MedicalRecordIndexData> GetIndexDataAsync(string userId, int petId = 0)
    {
        var pets = await _context.Pets.Where(p => p.OwnerID == userId).ToListAsync();

        var query = _context.MedicalRecords
            .Include(m => m.Pet)
            .Include(m => m.Vet)
            .Where(m => m.Pet!.OwnerID == userId)
            .OrderByDescending(m => m.VisitDate)
            .AsQueryable();

        if (petId > 0)
        {
            query = query.Where(r => r.PetID == petId);
        }

        return new MedicalRecordIndexData
        {
            Records = await query.ToListAsync(),
            Pets = pets,
            NoPet = !pets.Any(),
            SelectedPetId = petId
        };
    }

    public async Task<MedicalRecord?> GetDetailsAsync(int recordId)
    {
        return await _context.MedicalRecords
            .Include(m => m.Pet)
            .Include(m => m.Vet)
            .Include(m => m.RecordDiagnoses).ThenInclude(rd => rd.MedicalVocabularyDictionary)
            .Include(m => m.Prescriptions)
            .FirstOrDefaultAsync(m => m.RecordID == recordId);
    }

    public async Task<MedicalRecordCreateData> GetCreateDataAsync(string userId, int petId = 0)
    {
        var pets = await _context.Pets.Where(p => p.OwnerID == userId).ToListAsync();

        if (!pets.Any())
        {
            return new MedicalRecordCreateData { NoPet = true };
        }

        if (petId == 0)
        {
            petId = pets.First().PetID;
        }

        return new MedicalRecordCreateData
        {
            Pets = pets,
            SelectedPetId = petId,
            NoPet = false
        };
    }

    public async Task<ServiceResult<MedicalRecord>> CreateAsync(MedicalRecord medicalRecord, IFormFile? uploadFile)
    {
        var result = new ServiceResult<MedicalRecord> { IsSuccess = false };

        medicalRecord.VetID = DefaultVetId;

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

        var uniqueFileName = await SaveUploadFileAsync(uploadFile);
        medicalRecord.TestResultsFile = uniqueFileName;

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