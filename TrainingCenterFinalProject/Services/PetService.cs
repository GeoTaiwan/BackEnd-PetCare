using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class PetService : IPetService
{
    private readonly PetCareDbContext _context;
    private readonly IPetPhotoService _petPhotoService;

    public PetService(PetCareDbContext context, IPetPhotoService petPhotoService)
    {
        _context = context;
        _petPhotoService = petPhotoService;
    }

    public async Task<List<Pet>> GetUserPetsAsync(string userId)
    {
        return await _context.Pets
            .Where(p => p.OwnerID == userId)
            .ToListAsync();
    }

    public async Task<Pet?> GetDetailsAsync(int petId)
    {
        return await _context.Pets
            .Include(p => p.PetWeights.OrderByDescending(w => w.RecordDate))
            .Include(p => p.MedicalRecords.OrderByDescending(m => m.VisitDate))
            .Include(p => p.CareGroomingLogs.OrderByDescending(l => l.LogDate)).ThenInclude(l => l.Staff)
            .Include(p => p.Appointments.OrderByDescending(a => a.ApptDateTime)).ThenInclude(a => a.Institution)
            .FirstOrDefaultAsync(p => p.PetID == petId);
    }

    public async Task<Pet?> GetOwnedPetAsync(int petId, string ownerId)
    {
        return await _context.Pets
            .FirstOrDefaultAsync(p => p.PetID == petId && p.OwnerID == ownerId);
    }

    public async Task<ServiceResult<Pet>> CreateAsync(Pet pet, string ownerId, string webRootPath)
    {
        var result = new ServiceResult<Pet> { IsSuccess = false };

        pet.OwnerID = ownerId;
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        if (pet.UploadPhoto is not null && pet.UploadPhoto.Length > 0)
        {
            pet.ProfilePhotoUrl = await _petPhotoService.SavePetPhotoAsync(pet.UploadPhoto, ownerId, pet.PetID, webRootPath);
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();
        }

        result.IsSuccess = true;
        result.Data = pet;
        return result;
    }

    public async Task<ServiceResult<Pet>> UpdateAsync(int petId, Pet pet, string ownerId, string webRootPath)
    {
        var result = new ServiceResult<Pet> { IsSuccess = false };

        if (petId != pet.PetID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        var dbPet = await _context.Pets.FirstOrDefaultAsync(p => p.PetID == petId && p.OwnerID == ownerId);
        if (dbPet is null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        dbPet.Name = pet.Name;
        dbPet.Species = pet.Species;
        dbPet.Breed = pet.Breed;
        dbPet.Gender = pet.Gender;
        dbPet.DateOfBirth = pet.DateOfBirth;
        dbPet.MicrochipNo = pet.MicrochipNo;
        dbPet.SpecialNotes = pet.SpecialNotes;
        dbPet.OwnerID = ownerId;

        if (pet.UploadPhoto is not null && pet.UploadPhoto.Length > 0)
        {
            _petPhotoService.DeletePetPhoto(dbPet.ProfilePhotoUrl, webRootPath);
            dbPet.ProfilePhotoUrl = await _petPhotoService.SavePetPhotoAsync(pet.UploadPhoto, ownerId, dbPet.PetID, webRootPath);
        }
        else
        {
            dbPet.ProfilePhotoUrl = pet.ProfilePhotoUrl;
        }

        try
        {
            await _context.SaveChangesAsync();
            result.IsSuccess = true;
            result.Data = dbPet;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Pets.AnyAsync(p => p.PetID == petId);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }
            throw;
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int petId, string webRootPath)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var pet = await _context.Pets.FindAsync(petId);
        if (pet is null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        _petPhotoService.DeletePetPhoto(pet.ProfilePhotoUrl, webRootPath);

        var petWeights = await _context.PetWeight.Where(w => w.PetID == petId).ToListAsync();
        var medRecords = await _context.MedicalRecords.Where(m => m.PetID == petId).ToListAsync();

        if (petWeights.Any()) _context.PetWeight.RemoveRange(petWeights);
        if (medRecords.Any()) _context.MedicalRecords.RemoveRange(medRecords);

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }

    public async Task<ServiceResult<bool>> DeletePhotoAsync(int petId, string ownerId, string webRootPath)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetID == petId && p.OwnerID == ownerId);
        if (pet is null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        _petPhotoService.DeletePetPhoto(pet.ProfilePhotoUrl, webRootPath);
        pet.ProfilePhotoUrl = null;
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }

    public List<string> PopulateGenderList()
    {
        var result = new List<string>();
        result.AddRange("公","母","公(絕育)","母(絕育)","其它");
        return result;
    }

}