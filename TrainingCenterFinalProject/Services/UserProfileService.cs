using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class UserProfileService : IUserProfileService
{
    private readonly PetCareDbContext _context;

    public UserProfileService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserProfile>> GetAllAsync()
    {
        return await _context.UserProfiles
            .OrderBy(u => u.UserID)
            .ToListAsync();
    }

    public async Task<UserProfile?> GetByIdAsync(string userId)
    {
        return await _context.UserProfiles
            .Include(u=>u.Employments)
            .FirstOrDefaultAsync(m => m.UserID == userId);
    }

    public async Task<ServiceResult<UserProfile>> CreateAsync(UserProfile userProfile)
    {
        var result = new ServiceResult<UserProfile> { IsSuccess = false };

        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = userProfile;
        return result;
    }

    public async Task<ServiceResult<UserProfile>> UpdateAsync(string userId, UserProfile userProfile)
    {
        var result = new ServiceResult<UserProfile> { IsSuccess = false };

        if (userId != userProfile.UserID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        try
        {
            _context.Update(userProfile);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = userProfile;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.UserProfiles.AnyAsync(e => e.UserID == userProfile.UserID);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string userId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var userProfile = await _context.UserProfiles.FindAsync(userId);
        if (userProfile == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        var creds = await _context.UserCredentialDetails
            .Where(x => x.UserID == userId)
            .ToListAsync();

        if (creds.Any())
        {
            _context.UserCredentialDetails.RemoveRange(creds);
        }

        _context.UserProfiles.Remove(userProfile);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }
}