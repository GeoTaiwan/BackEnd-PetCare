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

    public SelectList PopulateGenderList()
    {
        var resultValue = new List<string>();
        resultValue.AddRange("公", "母", "公(絕育)", "母(絕育)", "其它");
        var resultName = new List<string>();
        resultName.AddRange("公(Male)", "母(Female)", "公(絕育)(Neutered Male)", "母(絕育)(Spayed Female)", "其它(Other)");
        var result = new List<SelectListItem>();
        for (int i = 0; i < resultName.Count; i++)
        {
            result.Add(new SelectListItem { Text = resultName[i], Value = resultValue[i] });
        }
        return new SelectList(result, "Value", "Text");
    }

    public SelectList PopulateSpeciesList()
    {
        var resultValue = new List<string>();
        resultValue.AddRange("狗", "貓", "兔子", "鼠/貂 (小型哺乳類)", "鳥類", "爬蟲/兩棲類", "其他");
        var resultName = new List<string>();
        resultName.AddRange("狗(Dog)", "貓(Cat)", "兔子(Rabbit)", "鼠/貂(小型哺乳類)", "鳥類(Bird)", "爬蟲/兩棲類(Reptile/Amphibian)", "其他(Other)");
        var result = new List<SelectListItem>();
        for (int i = 0; i < resultName.Count; i++)
        {
            result.Add(new SelectListItem { Text = resultName[i], Value = resultValue[i] });
        }
        return new SelectList(result, "Value", "Text");
    }
    public Dictionary<string, List<string>> GetPetBreedDictionary()
    {
        return new Dictionary<string, List<string>>
    {
        { "狗", new List<string> { "米克斯 (Mix)", "柴犬", "貴賓犬", "馬爾濟斯", "柯基", "黃金獵犬", "法國鬥牛犬", "博美犬", "其他 (自行輸入)" } },
        { "貓", new List<string> { "米克斯 (Mix)", "英國短毛貓", "布偶貓", "美國短毛貓", "波斯貓", "曼赤肯", "俄羅斯藍貓", "其他 (自行輸入)" } },
        { "兔子", new List<string> { "獅子兔", "侏儒兔", "垂耳兔", "道奇兔", "安哥拉兔", "其他 (自行輸入)" } },
        { "鼠/貂 (小型哺乳類)", new List<string> { "倉鼠 (三線/一線/老公公)", "黃金鼠", "天竺鼠 (豚鼠)", "龍貓 (絨鼠)", "雪貂", "蜜袋鼯", "刺蝟", "其他 (自行輸入)" } },
        { "鳥類", new List<string> { "虎皮鸚鵡", "玄鳳鸚鵡", "和尚鸚鵡", "金剛鸚鵡", "灰鸚鵡", "文鳥", "金絲雀", "其他 (自行輸入)" } },
        { "爬蟲/兩棲類", new List<string> { "豹紋守宮", "肥尾守宮", "睫角守宮", "鬆獅蜥", "陸龜 (如赫曼/蘇卡達)", "澤龜 (如斑龜/巴西龜)", "球蟒", "豬鼻蛇", "角蛙", "其他 (自行輸入)" } },
        { "其他", new List<string> { "其他 (自行輸入)" } }
    };
    }

    public async Task<bool> UpdatePetWeightAsync(PetWeight newWeight)
    {
        if (newWeight == null)
        {
            return false;
        }
        try
        {
            _context.PetWeight.Add(newWeight);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return false;
        }
        return true;
    }

}