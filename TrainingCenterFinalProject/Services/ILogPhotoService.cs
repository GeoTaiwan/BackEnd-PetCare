using Microsoft.AspNetCore.Http;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface ILogPhotoService
{
    Task<List<LogPhoto>> GetPhotosAsync(int logId = 0);
    Task<LogPhoto?> GetByIdAsync(int photoId);

    Task<ServiceResult<LogPhoto>> CreateAsync(LogPhoto logPhoto);
    Task<ServiceResult<LogPhoto>> UpdateAsync(int photoId, LogPhoto logPhoto);

    Task<ServiceResult<bool>> DeleteAsync(int photoId);
    Task<ServiceResult<int>> UploadPhotosAsync(int logId, List<IFormFile> uploadedFiles);
    Task<ServiceResult<bool>> DeletePhotoWithFileAsync(int photoId);
}