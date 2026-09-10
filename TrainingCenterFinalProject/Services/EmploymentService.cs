using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.ViewModels;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class EmploymentService : IEmploymentService
{
    private readonly PetCareDbContext _context;

    public EmploymentService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetInstitutionIdByUserIdAsync(string userId)
    {
        return await _context.Employments
            .Where(e => e.UserID == userId)
            .Select(e => e.InstitutionID)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetInstitutionNameAsync(string institutionId)
    {
        return await _context.Institutions
            .Where(i => i.InstitutionID == institutionId)
            .Select(i => i.Name)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Employment>> GetEmploymentsByInstitutionAsync(string institutionId)
    {
        return await _context.Employments
            .Include(e => e.UserProfile)
            .Include(e => e.OccupationDetails)
            .Include(e => e.Institution)
            .Where(e => e.InstitutionID == institutionId)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<NewEmployeeViewModel?> BuildCreateViewModelAsync(string institutionId)
    {
        var inst = await _context.Institutions.FindAsync(institutionId);
        if (inst == null) return null;

        return new NewEmployeeViewModel
        {
            InstitutionID = institutionId,
            InstitutionName = inst.Name,
            Institution = inst,
            LicenseNumber = "待填"
        };
    }

    public async Task<List<SelectListItem>> GetOccupationSelectListAsync()
    {
        return await _context.Occupations
            .Select(o => new SelectListItem
            {
                Value = o.OccupationID,
                Text = o.OccupationName
            })
            .ToListAsync();
    }

    public List<SelectListItem> GetStatusSelectList()
    {
        return new List<SelectListItem>
        {
            new() { Value = "Active", Text = "在職 (Active)" },
            new() { Value = "Inactive", Text = "離職 (Inactive)" },
            new() { Value = "Suspended", Text = "停職 (Suspended)" }
        };
    }

    public async Task<ServiceResult<Employment>> CreateAsync(NewEmployeeViewModel model)
    {
        var result = new ServiceResult<Employment> { IsSuccess = false };

        var userId = await _context.UserProfiles
            .Where(u => u.Email == model.UserEmail)
            .Select(u => u.UserID)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(userId))
        {
            result.ErrorMessage = "USER_NOT_FOUND";
            return result;
        }

        var exists = await _context.Employments.AnyAsync(e =>
            e.UserID == userId && e.InstitutionID == model.InstitutionID);

        if (exists)
        {
            result.ErrorMessage = "EMPLOYMENT_EXISTS";
            return result;
        }

        var employment = new Employment
        {
            UserID = userId,
            InstitutionID = model.InstitutionID,
            OccupationID = model.OccupationID,
            Status = model.Status,
            LicenseNumber = model.LicenseNumber,
            StartDate = DateTime.Now
        };

        var user = await _context.UserProfiles.FindAsync(userId);
        if (user == null)
        {
            result.ErrorMessage = "USER_NOT_FOUND";
            return result;
        }

        user.Authorization = (byte) (employment.OccupationID == "O00" ? 4 : 1);

        _context.Employments.Add(employment);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = employment;
        return result;
    }

    public async Task<Employment?> GetEmploymentForEditAsync(string currentUserId, string targetUserId)
    {
        var instId = await GetInstitutionIdByUserIdAsync(currentUserId);
        if (string.IsNullOrEmpty(instId)) return null;

        return await _context.Employments
            .Include(e => e.Institution)
            .Include(e => e.UserProfile)
            .FirstOrDefaultAsync(e => e.UserID == targetUserId && e.InstitutionID == instId);
    }

    public async Task<ServiceResult<Employment>> UpdateAsync(string currentUserId, string targetUserId, Employment employment)
    {
        var result = new ServiceResult<Employment> { IsSuccess = false };

        if (targetUserId != employment.UserID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        var instId = await GetInstitutionIdByUserIdAsync(currentUserId);
        if (string.IsNullOrEmpty(instId))
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        var dbEmployment = await _context.Employments
            .FirstOrDefaultAsync(e => e.UserID == targetUserId && e.InstitutionID == instId);

        if (dbEmployment == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        dbEmployment.OccupationID = employment.OccupationID;
        dbEmployment.Status = employment.Status;
        dbEmployment.LicenseNumber = employment.LicenseNumber;
        dbEmployment.StartDate = employment.StartDate;

        var user = await _context.UserProfiles.FindAsync(dbEmployment.UserID);
        if (user == null)
        {
            result.ErrorMessage = "USER_NOT_FOUND";
            return result;
        }

        if (dbEmployment.Status == "Inactive")
        {
            dbEmployment.EndDate = DateTime.Now;
            user.Authorization = 0;
        }
        else if (dbEmployment.Status == "Active")
        {
            dbEmployment.EndDate = null;
            user.Authorization = (byte) (dbEmployment.OccupationID == "O00" ? 4 : 1);
        }
        else
        {
            user.Authorization = 0;
        }

        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = dbEmployment;
        return result;
    }

    public async Task<Employment?> GetByUserIdAsync(string userId)
    {
        return await _context.Employments.FirstOrDefaultAsync(e => e.UserID == userId);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string userId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var employment = await _context.Employments.FindAsync(userId);
        if (employment == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        _context.Employments.Remove(employment);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }
}