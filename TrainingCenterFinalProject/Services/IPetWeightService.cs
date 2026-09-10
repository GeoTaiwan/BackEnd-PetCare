using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IPetWeightService
{
    Task<List<PetWeight>> GetByPetForOwnerAsync(int petId, string ownerUserId);
    Task<ServiceResult<PetWeight>> CreateAsync(PetWeight petWeight);
    Task<PetWeight?> GetByIdAsync(int petWeightId);
    Task<ServiceResult<PetWeight>> UpdateWeightAsync(int petWeightId, decimal weight);
    Task<ServiceResult<int>> DeleteAsync(int petWeightId);
}