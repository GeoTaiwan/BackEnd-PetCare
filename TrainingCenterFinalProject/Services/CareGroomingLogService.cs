using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class CareGroomingLogService : ICareGroomingLogService
{
    private readonly PetCareDbContext _context;

    public CareGroomingLogService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<CareGroomingLog>> GetLogsForUserAsync(string userId)
    {
        var isStaff = await IsStaffUserAsync(userId);

        if (isStaff)
        {
            return await _context.CareGroomingLogs
                .Where(cg => cg.StaffID == userId)
                .Include(cg => cg.Pet).ThenInclude(p => p.Owner)
                .Include(cg => cg.Staff)
                .ToListAsync();
        }

        return await _context.CareGroomingLogs
            .Include(cg => cg.Pet).ThenInclude(p => p.Owner)
            .Include(cg => cg.Staff)
            .Where(cg => cg.Pet!.OwnerID == userId)
            .ToListAsync();
    }

    public async Task<CareGroomingLog?> GetDetailsAsync(int logId)
    {
        return await _context.CareGroomingLogs
            .Include(cg => cg.Pet)
            .Include(cg => cg.Staff)
            .Include(cg => cg.LogPhotos)
            .FirstOrDefaultAsync(m => m.LogID == logId);
    }

    public async Task<CareGroomingLogCreateData> GetCreateDataAsync(string userId, int petId)
    {
        var userPets = await _context.Pets
            .Where(p => p.OwnerID == userId)
            .ToListAsync();

        if (petId == 0)
        {
            if (!userPets.Any())
            {
                return new CareGroomingLogCreateData
                {
                    ShouldRedirectToPets = true,
                    ErrorMessage = "您尚未新增寵物，請先新增寵物後再新增照護紀錄。"
                };
            }

            petId = userPets.First().PetID;
        }

        return new CareGroomingLogCreateData
        {
            Log = new CareGroomingLog
            {
                PetID = petId
            }
        };
    }

    public async Task<ServiceResult<CareGroomingLog>> CreateAsync(CareGroomingLog log, string currentUserId)
    {
        var result = new ServiceResult<CareGroomingLog> { IsSuccess = false };

        log.StaffID = currentUserId;
        if (log.LogDate == default) log.LogDate = DateTime.Now;

        _context.Add(log);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = log;
        return result;
    }

    public async Task<CareGroomingLog?> GetEditDataAsync(int logId)
    {
        return await _context.CareGroomingLogs
            .Include(cg => cg.Staff)
            .Include(cg => cg.Pet)
            .Include(cg => cg.LogPhotos)
            .FirstOrDefaultAsync(cg => cg.LogID == logId);
    }

    public async Task<ServiceResult<CareGroomingLog>> UpdateAsync(int logId, CareGroomingLog log, string currentUserId)
    {
        var result = new ServiceResult<CareGroomingLog> { IsSuccess = false };

        if (logId != log.LogID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        var dbLog = await _context.CareGroomingLogs.FirstOrDefaultAsync(x => x.LogID == logId);
        if (dbLog == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        if (dbLog.StaffID != currentUserId)
        {
            result.ErrorMessage = "FORBIDDEN";
            return result;
        }

        dbLog.ActivityDetails = log.ActivityDetails;
        dbLog.DietNotes = log.DietNotes;

        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = dbLog;
        return result;
    }

    public async Task<CareGroomingLog?> GetDeleteDataAsync(int logId)
    {
        return await _context.CareGroomingLogs
            .FirstOrDefaultAsync(m => m.LogID == logId);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int logId, string currentUserId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var log = await _context.CareGroomingLogs.FirstOrDefaultAsync(x => x.LogID == logId);
        if (log == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        if (log.StaffID != currentUserId)
        {
            result.ErrorMessage = "FORBIDDEN";
            return result;
        }

        _context.CareGroomingLogs.Remove(log);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }

    private async Task<bool> IsStaffUserAsync(string userId)
    {
        var userEmployments = await _context.Employments
            .Where(e => e.UserID == userId)
            .ToListAsync();

        return userEmployments.Any(e =>
            !string.IsNullOrEmpty(e.OccupationID) &&
            e.OccupationID.Length >= 3 &&
            int.TryParse(e.OccupationID.Substring(1, 2), out int occId) &&
            occId > 10);
    }
}