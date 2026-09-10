using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class PetWeightService : IPetWeightService
{
    private readonly PetCareDbContext _context;

    public PetWeightService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<PetWeight>> GetByPetForOwnerAsync(int petId, string ownerUserId)
    {
        return await _context.PetWeight
            .Include(pw => pw.Pet)
            .Where(pw => pw.PetID == petId && pw.Pet!.OwnerID == ownerUserId)
            .OrderByDescending(pw => pw.RecordDate)
            .ToListAsync();
    }

    public async Task<ServiceResult<PetWeight>> CreateAsync(PetWeight petWeight)
    {
        var result = new ServiceResult<PetWeight> { IsSuccess = false };

        petWeight.RecordDate = DateTime.Now;
        _context.PetWeight.Add(petWeight);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = petWeight;
        return result;
    }

    public async Task<PetWeight?> GetByIdAsync(int petWeightId)
    {
        return await _context.PetWeight.FindAsync(petWeightId);
    }

    public async Task<ServiceResult<PetWeight>> UpdateWeightAsync(int petWeightId, decimal weight)
    {
        var result = new ServiceResult<PetWeight> { IsSuccess = false };

        var dbRecord = await _context.PetWeight.FindAsync(petWeightId);
        if (dbRecord == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        dbRecord.Weight = weight;

        try
        {
            _context.Update(dbRecord);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = dbRecord;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.PetWeight.AnyAsync(e => e.PetWeightID == petWeightId);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<ServiceResult<int>> DeleteAsync(int petWeightId)
    {
        var result = new ServiceResult<int> { IsSuccess = false };

        var petWeight = await _context.PetWeight.FindAsync(petWeightId);
        if (petWeight == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        int petId = petWeight.PetID;
        _context.PetWeight.Remove(petWeight);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = petId;
        return result;
    }
}