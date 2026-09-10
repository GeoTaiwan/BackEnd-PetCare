using Microsoft.AspNetCore.Http;

namespace TrainingCenterFinalProject.Services;

public class PetPhotoService : IPetPhotoService
{
    public async Task<string> SavePetPhotoAsync(IFormFile file, string ownerId, int petId, string webRootPath)
    {
        var uploadsFolder = Path.Combine(webRootPath, "uploads", "pets");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var extension = Path.GetExtension(file.FileName);
        var newFileName = $"{ownerId}_{petId}{extension}";
        var filePath = Path.Combine(uploadsFolder, newFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return "/uploads/pets/" + newFileName;
    }

    public void DeletePetPhoto(string? profilePhotoUrl, string webRootPath)
    {
        if (string.IsNullOrWhiteSpace(profilePhotoUrl)) return;

        var uploadsFolder = Path.Combine(webRootPath, "uploads", "pets");
        var fileName = Path.GetFileName(profilePhotoUrl);
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}