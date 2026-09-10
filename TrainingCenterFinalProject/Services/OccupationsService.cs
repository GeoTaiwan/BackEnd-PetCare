using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class OccupationsService : IOccupationsService
{
    private readonly PetCareDbContext _context;

    public OccupationsService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<Occupations>> GetAllAsync()
    {
        return await _context.Occupations
            .OrderBy(o => o.OccupationID)
            .ToListAsync();
    }

    public async Task<Occupations?> GetByIdAsync(string occupationId)
    {
        return await _context.Occupations
            .FirstOrDefaultAsync(m => m.OccupationID == occupationId);
    }

    public async Task<ServiceResult<Occupations>> CreateAsync(Occupations occupations)
    {
        var result = new ServiceResult<Occupations> { IsSuccess = false };

        _context.Occupations.Add(occupations);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = occupations;
        return result;
    }

    public async Task<ServiceResult<Occupations>> UpdateAsync(string occupationId, Occupations occupations)
    {
        var result = new ServiceResult<Occupations> { IsSuccess = false };

        if (occupationId != occupations.OccupationID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        try
        {
            _context.Update(occupations);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = occupations;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Occupations.AnyAsync(e => e.OccupationID == occupations.OccupationID);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string occupationId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var occupations = await _context.Occupations.FindAsync(occupationId);
        if (occupations == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        _context.Occupations.Remove(occupations);

        try
        {
            await _context.SaveChangesAsync();
            result.IsSuccess = true;
            result.Data = true;
            return result;
        }
        catch (DbUpdateException)
        {
            result.ErrorMessage = "IN_USE";
            return result;
        }
    }
}