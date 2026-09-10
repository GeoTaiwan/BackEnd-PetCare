using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IPetService
{
    Task<List<Pet>> GetUserPetsAsync(string userId);
    Task<Pet?> GetDetailsAsync(int petId);
    Task<Pet?> GetOwnedPetAsync(int petId, string ownerId);

    Task<ServiceResult<Pet>> CreateAsync(Pet pet, string ownerId, string webRootPath);
    Task<ServiceResult<Pet>> UpdateAsync(int petId, Pet pet, string ownerId, string webRootPath);
    Task<ServiceResult<bool>> DeleteAsync(int petId, string webRootPath);
    Task<ServiceResult<bool>> DeletePhotoAsync(int petId, string ownerId, string webRootPath);
    List<string> PopulateGenderList();
}