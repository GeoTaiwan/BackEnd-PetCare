using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class InstitutionService : IInstitutionService
{
    private readonly PetCareDbContext _context;

    public InstitutionService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<Institution>> GetMyInstitutionsAsync(string ownerId)
    {
        return await _context.Institutions
            .Include(i => i.InstitutionStatus)
            .Include(i => i.InstitutionType)
            .Include(i => i.Owner)
            .Where(i => i.OwnerID == ownerId)
            .ToListAsync();
    }

    public async Task<List<Institution>> GetInstitutionsForReservationAsync(string? typeId)
    {
        var query = _context.Institutions.AsQueryable();

        if (!string.IsNullOrEmpty(typeId))
        {
            query = query.Where(i => i.TypeId == typeId);
        }

        return await query.ToListAsync();
    }

    public async Task<Institution?> GetInstitutionManageAsync(string institutionId)
    {
        return await _context.Institutions
            .Include(i => i.InstitutionStatus)
            .Include(i => i.InstitutionType)
            .Include(i => i.Owner)
            .Include(i => i.Employments)
            .ThenInclude(e => e.UserProfile)
            .FirstOrDefaultAsync(i => i.InstitutionID == institutionId);
    }

    public async Task<Institution?> GetDetailsAsync(string institutionId)
    {
        return await _context.Institutions
            .Include(i => i.InstitutionStatus)
            .Include(i => i.InstitutionType)
            .Include(i => i.Owner)
            .FirstOrDefaultAsync(i => i.InstitutionID == institutionId);
    }

    public async Task<InstitutionCreateCheckResult> CheckCreateEligibilityAsync(string userId)
    {
        var pending = await _context.Institutions
            .Where(i => i.OwnerID == userId && i.StatusId == "S00")
            .Select(i => i.InstitutionID)
            .FirstOrDefaultAsync();

        return new InstitutionCreateCheckResult
        {
            HasPendingApplication = !string.IsNullOrEmpty(pending),
            PendingInstitutionId = pending
        };
    }

    public async Task<ServiceResult<Institution>> CreateAsync(Institution institution, string currentUserId)
    {
        var result = new ServiceResult<Institution> { IsSuccess = false };

        bool alreadyOwns = await _context.Employments.AnyAsync(e =>
            e.UserID == currentUserId &&
            e.OccupationID == "O00" &&
            e.Status == "Active");

        if (alreadyOwns)
        {
            result.ErrorMessage = "ALREADY_OWNER";
            return result;
        }

        institution.OwnerID = currentUserId;
        institution.StatusId = "S00";

        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            institution.InstitutionID = await GenerateInstitutionIdAsync();

            await using var tx = await _context.Database.BeginTransactionAsync();
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
                await tx.CommitAsync();

                result.IsSuccess = true;
                result.Data = institution;
                return result;
            }
            catch (DbUpdateException) when (attempt < maxRetries - 1)
            {
                await tx.RollbackAsync();
                _context.Entry(institution).State = EntityState.Detached;
            }
        }

        result.ErrorMessage = "RETRY_FAILED";
        return result;
    }

    public async Task<Institution?> GetByIdAsync(string institutionId)
    {
        return await _context.Institutions.FindAsync(institutionId);
    }

    public async Task<ServiceResult<Institution>> UpdateAsync(Institution institution, bool fromDetails, string currentUserId)
    {
        var result = new ServiceResult<Institution> { IsSuccess = false };

        if (fromDetails)
        {
            var myInstitutionId = await _context.Institutions
                .Where(i => i.OwnerID == currentUserId)
                .Select(i => i.InstitutionID)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(myInstitutionId))
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            institution.InstitutionID = myInstitutionId;
            institution.OwnerID = currentUserId;
            institution.StatusId = "S00";
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
            var exists = await _context.Institutions.AnyAsync(i => i.InstitutionID == institution.InstitutionID);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }
            throw;
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string institutionId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var appointments = await _context.Appointments.Where(a => a.InstitutionID == institutionId).ToListAsync();
        if (appointments.Any()) _context.Appointments.RemoveRange(appointments);

        var authorizations = await _context.AuthorizationRecords.Where(a => a.InstitutionID == institutionId).ToListAsync();
        if (authorizations.Any()) _context.AuthorizationRecords.RemoveRange(authorizations);

        var employments = await _context.Employments.Where(e => e.InstitutionID == institutionId).ToListAsync();
        if (employments.Any()) _context.Employments.RemoveRange(employments);

        var taskSchedules = await _context.TaskSchedules.Where(t => t.InstitutionID == institutionId).ToListAsync();
        if (taskSchedules.Any()) _context.TaskSchedules.RemoveRange(taskSchedules);

        await _context.SaveChangesAsync();

        var institution = await _context.Institutions.FindAsync(institutionId);
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

    public async Task<List<SelectListItem>> GetInstitutionTypeSelectListAsync()
    {
        return await _context.InstitutionTypes
            .Select(t => new SelectListItem { Value = t.TypeId, Text = t.Name })
            .ToListAsync();
    }

    public async Task<List<SelectListItem>> GetInstitutionStatusSelectListAsync()
    {
        return await _context.InstitutionStatuses
            .Select(s => new SelectListItem { Value = s.StatusId, Text = s.StatusName })
            .ToListAsync();
    }

    private async Task<string> GenerateInstitutionIdAsync()
    {
        var last = await _context.Institutions
            .OrderByDescending(i => i.InstitutionID)
            .Select(i => i.InstitutionID)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(last)) return "I00001";

        int num = int.Parse(last[1..]);
        return $"I{(num + 1):D5}";
    }
}