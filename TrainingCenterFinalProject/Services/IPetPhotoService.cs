using Microsoft.AspNetCore.Http;

namespace TrainingCenterFinalProject.Services;

public interface IPetPhotoService
{
    Task<string> SavePetPhotoAsync(IFormFile file, string ownerId, int petId, string webRootPath);
    void DeletePetPhoto(string? profilePhotoUrl, string webRootPath);
}