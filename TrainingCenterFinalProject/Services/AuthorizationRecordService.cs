using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class AuthorizationRecordService : IAuthorizationRecordService
{
    private readonly PetCareDbContext _context;

    public AuthorizationRecordService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuthorizationRecord>> GetUserRecordsAsync(string userId)
    {
        return await _context.AuthorizationRecords
            .Include(ar => ar.Pet)
            .Include(ar => ar.Institution)
            .Where(ar => ar.Pet!.OwnerID == userId)
            .OrderByDescending(ar => ar.GrantedDate)
            .ToListAsync();
    }

    public async Task<bool> ChangeStatusAsync(string dataAuthorizationNumber, bool isAuthorized)
    {
        var record = await _context.AuthorizationRecords
            .FirstOrDefaultAsync(m => m.DataAuthorizationNumber == dataAuthorizationNumber);

        if (record == null) return false;

        record.Status = isAuthorized ? "Authorized" : "Revoked";
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AuthorizationRecord?> GetByIdAsync(string dataAuthorizationNumber)
    {
        return await _context.AuthorizationRecords
            .FirstOrDefaultAsync(m => m.DataAuthorizationNumber == dataAuthorizationNumber);
    }

    public async Task<AuthorizationRecordCreateData?> GetCreateDataAsync(int petId, string institutionId)
    {
        if (petId <= 0 || string.IsNullOrWhiteSpace(institutionId)) return null;

        var petName = await _context.Pets
            .Where(p => p.PetID == petId)
            .Select(p => p.Name)
            .FirstOrDefaultAsync();

        var instName = await _context.Institutions
            .Where(i => i.InstitutionID == institutionId)
            .Select(i => i.Name)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(petName) || string.IsNullOrWhiteSpace(instName)) return null;

        return new AuthorizationRecordCreateData
        {
            Record = new AuthorizationRecord
            {
                PetID = petId,
                InstitutionID = institutionId
            },
            PetName = petName,
            InstitutionName = instName
        };
    }

    public async Task<string?> GetInstitutionIdByOwnerAsync(string ownerId)
    {
        return await _context.Institutions
            .Where(i => i.OwnerID == ownerId)
            .Select(i => i.InstitutionID)
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<AuthorizationRecord>> CreateAsync(AuthorizationRecord authorizationRecord)
    {
        var result = new ServiceResult<AuthorizationRecord> { IsSuccess = false };

        if (string.IsNullOrWhiteSpace(authorizationRecord.InstitutionID))
        {
            result.ErrorMessage = "InstitutionID 不可為空。";
            return result;
        }

        authorizationRecord.DataAuthorizationNumber = await GenerateAuthRecordIdAsync(authorizationRecord.InstitutionID);
        authorizationRecord.Status = "Authorized";

        _context.AuthorizationRecords.Add(authorizationRecord);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = authorizationRecord;
        return result;
    }

    public async Task<ServiceResult<AuthorizationRecord>> CreatePendingAsync(AuthorizationRecord authorizationRecord)
    {
        var result = new ServiceResult<AuthorizationRecord> { IsSuccess = false };

        if (string.IsNullOrWhiteSpace(authorizationRecord.InstitutionID))
        {
            result.ErrorMessage = "InstitutionID 不可為空。";
            return result;
        }

        authorizationRecord.DataAuthorizationNumber = await GenerateAuthRecordIdAsync(authorizationRecord.InstitutionID);
        authorizationRecord.Status = "Pending";

        if (authorizationRecord.RevokedDate.HasValue)
        {
            authorizationRecord.RevokedDate = authorizationRecord.RevokedDate.Value.Date
                .AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        _context.AuthorizationRecords.Add(authorizationRecord);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = authorizationRecord;
        return result;
    }

    public async Task<ServiceResult<AuthorizationRecord>> UpdateAsync(string dataAuthorizationNumber, AuthorizationRecord authorizationRecord)
    {
        var result = new ServiceResult<AuthorizationRecord> { IsSuccess = false };

        if (dataAuthorizationNumber != authorizationRecord.DataAuthorizationNumber)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        try
        {
            _context.Update(authorizationRecord);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = authorizationRecord;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.AuthorizationRecords
                .AnyAsync(e => e.DataAuthorizationNumber == authorizationRecord.DataAuthorizationNumber);

            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<bool> DeleteAsync(string dataAuthorizationNumber)
    {
        var record = await _context.AuthorizationRecords.FindAsync(dataAuthorizationNumber);
        if (record == null) return false;

        _context.AuthorizationRecords.Remove(record);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> GenerateAuthRecordIdAsync(string instID)
    {
        string datePart = DateTime.Now.ToString("yyyyMMdd");

        var lastRecord = await _context.AuthorizationRecords
            .Where(ar => ar.DataAuthorizationNumber.StartsWith($"R{datePart}-{instID}-"))
            .OrderByDescending(ar => ar.DataAuthorizationNumber)
            .FirstOrDefaultAsync();

        if (lastRecord == null || string.IsNullOrWhiteSpace(lastRecord.DataAuthorizationNumber))
        {
            return $"R{datePart}-{instID}-001";
        }

        string[] parts = lastRecord.DataAuthorizationNumber.Split('-');
        if (parts.Length >= 3 && int.TryParse(parts.Last(), out int lastNumber))
        {
            return $"R{datePart}-{instID}-{(lastNumber + 1):D3}";
        }

        return $"R{datePart}-{instID}-001";
    }
}