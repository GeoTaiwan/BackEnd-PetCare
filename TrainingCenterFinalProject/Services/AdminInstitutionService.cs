using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class AdminInstitutionService : IAdminInstitutionService
{
    private readonly PetCareDbContext _context;

    public AdminInstitutionService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<Institution>> GetAllAsync()
    {
        return await _context.Institutions
            .Include(i => i.InstitutionStatus)
            .Include(i => i.InstitutionType)
            .Include(i => i.Owner)
            .ToListAsync();
    }

    public async Task<Institution?> GetDetailsAsync(string id)
    {
        return await _context.Institutions
            .Include(i => i.InstitutionStatus)
            .Include(i => i.InstitutionType)
            .Include(i => i.Owner)
            .FirstOrDefaultAsync(m => m.InstitutionID == id);
    }

    public async Task<Institution?> GetByIdAsync(string id)
    {
        return await _context.Institutions.FindAsync(id);
    }

    public async Task<Institution?> GetPendingInstitutionByOwnerAsync(string ownerId)
    {
        return await _context.Institutions
            .FirstOrDefaultAsync(i => i.OwnerID == ownerId && i.StatusId == "S00");
    }

    public async Task<ServiceResult<Institution>> CreateInstitutionAsync(Institution institution, string currentUserId)
    {
        var result = new ServiceResult<Institution> { IsSuccess = false };

        bool alreadyOwns = await _context.Employments.AnyAsync(e =>
            e.UserID == currentUserId &&
            e.OccupationID == "O00" &&
            e.Status == "Active");

        if (alreadyOwns)
        {
            result.ErrorMessage = "您已經是某機構的負責人，無法重複申請。";
            return result;
        }

        institution.StatusId = "S00";
        institution.OwnerID = currentUserId;

        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            institution.InstitutionID = await GenerateInstitutionIDAsync();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Institutions.Add(institution);
                await _context.SaveChangesAsync();

                _context.Employments.Add(new Employment
                {
                    UserID = currentUserId,
                    InstitutionID = institution.InstitutionID,
                    OccupationID = "O00",
                    Status = "Active",
                    LicenseNumber = institution.LicenseNumber ?? "PENDING",
                    StartDate = DateTime.Now
                });
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                result.IsSuccess = true;
                result.Data = institution;
                return result;
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                _context.Entry(institution).State = EntityState.Detached;

                if (attempt == maxRetries - 1)
                {
                    result.ErrorMessage = "系統忙碌，請稍後再試一次。";
                    return result;
                }
            }
        }

        result.ErrorMessage = "系統忙碌，請稍後再試一次。";
        return result;
    }

    public async Task<ServiceResult<Institution>> UpdateInstitutionAsync(Institution institution, bool fromDetails, string currentUserId)
    {
        var result = new ServiceResult<Institution> { IsSuccess = false };

        if (fromDetails)
        {
            institution.StatusId = "S00";
            institution.OwnerID = currentUserId;

            institution.InstitutionID = await _context.Institutions
                .Where(i => i.OwnerID == currentUserId)
                .Select(i => i.InstitutionID)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(institution.InstitutionID))
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }
        }

        var owner = await _context.UserProfiles.FindAsync(institution.OwnerID);

        var activeInstitutions = await _context.Institutions
            .Where(i => i.OwnerID == institution.OwnerID
                        && i.StatusId == "S01"
                        && i.InstitutionID != institution.InstitutionID)
            .ToListAsync();

        if (institution.StatusId == "S01")
        {
            if (owner != null) owner.Authorization = 2;
        }
        else if (!activeInstitutions.Any() && owner != null)
        {
            owner.Authorization = 1;
        }
        else
        {
            var activeEmployments = await _context.Employments
                .Where(e => e.UserID == institution.OwnerID && e.Status == "Active")
                .ToListAsync();

            if (!activeEmployments.Any() && owner != null)
            {
                owner.Authorization = 0;
            }
        }

        try
        {
            _context.Update(institution);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = institution;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            bool exists = await _context.Institutions.AnyAsync(e => e.InstitutionID == institution.InstitutionID);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<ServiceResult<bool>> DeleteInstitutionAsync(string id)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var appointments = await _context.Appointments.Where(a => a.InstitutionID == id).ToListAsync();
        if (appointments.Any()) _context.Appointments.RemoveRange(appointments);

        var authorizations = await _context.AuthorizationRecords.Where(a => a.InstitutionID == id).ToListAsync();
        if (authorizations.Any()) _context.AuthorizationRecords.RemoveRange(authorizations);

        var employments = await _context.Employments.Where(e => e.InstitutionID == id).ToListAsync();
        if (employments.Any()) _context.Employments.RemoveRange(employments);

        var taskAssignments = await _context.TaskSchedules.Where(t => t.InstitutionID == id).ToListAsync();
        if (taskAssignments.Any()) _context.TaskSchedules.RemoveRange(taskAssignments);

        await _context.SaveChangesAsync();

        var institution = await _context.Institutions.FindAsync(id);
        if (institution == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        _context.Institutions.Remove(institution);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }

    public async Task<List<InstitutionType>> GetInstitutionTypesAsync()
    {
        return await _context.InstitutionTypes.ToListAsync();
    }

    public async Task<List<InstitutionStatus>> GetInstitutionStatusesAsync()
    {
        return await _context.InstitutionStatuses.ToListAsync();
    }

    private async Task<string> GenerateInstitutionIDAsync()
    {
        var lastInstitution = await _context.Institutions
            .OrderByDescending(i => i.InstitutionID)
            .FirstOrDefaultAsync();

        if (lastInstitution == null) return "I00001";

        int lastIdNumber = int.Parse(lastInstitution.InstitutionID.Substring(1));
        return "I" + (lastIdNumber + 1).ToString("D5");
    }
}