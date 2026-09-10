using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class LogPhotoService : ILogPhotoService
{
    private readonly PetCareDbContext _context;
    private readonly IWebHostEnvironment _env;

    public LogPhotoService(PetCareDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<List<LogPhoto>> GetPhotosAsync(int logId = 0)
    {
        var query = _context.LogPhotos.AsQueryable();

        if (logId > 0)
        {
            query = query.Where(p => p.LogID == logId);
        }

        return await query
            .Include(lp => lp.CareGroomingLog)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<LogPhoto?> GetByIdAsync(int photoId)
    {
        return await _context.LogPhotos.FirstOrDefaultAsync(m => m.PhotoID == photoId);
    }

    public async Task<ServiceResult<LogPhoto>> CreateAsync(LogPhoto logPhoto)
    {
        var result = new ServiceResult<LogPhoto> { IsSuccess = false };

        if (string.IsNullOrWhiteSpace(logPhoto.ImageURL))
        {
            result.ErrorMessage = "ImageURL 為必填。";
            return result;
        }

        logPhoto.CreatedDate = DateTime.Now;
        _context.LogPhotos.Add(logPhoto);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = logPhoto;
        return result;
    }

    public async Task<ServiceResult<LogPhoto>> UpdateAsync(int photoId, LogPhoto logPhoto)
    {
        var result = new ServiceResult<LogPhoto> { IsSuccess = false };

        if (photoId != logPhoto.PhotoID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        try
        {
            _context.Update(logPhoto);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = logPhoto;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.LogPhotos.AnyAsync(e => e.PhotoID == logPhoto.PhotoID);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int photoId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var logPhoto = await _context.LogPhotos.FindAsync(photoId);
        if (logPhoto == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        _context.LogPhotos.Remove(logPhoto);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }

    public async Task<ServiceResult<int>> UploadPhotosAsync(int logId, List<IFormFile> uploadedFiles)
    {
        var result = new ServiceResult<int> { IsSuccess = false };

        if (uploadedFiles == null || uploadedFiles.Count == 0)
        {
            result.ErrorMessage = "NO_FILE";
            return result;
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "logPhotos");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        int count = 0;
        foreach (var file in uploadedFiles.Where(f => f.Length > 0))
        {
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            _context.LogPhotos.Add(new LogPhoto
            {
                LogID = logId,
                ImageURL = "/uploads/logPhotos/" + uniqueFileName,
                CreatedDate = DateTime.Now
            });

            count++;
        }

        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = count;
        return result;
    }

    public async Task<ServiceResult<bool>> DeletePhotoWithFileAsync(int photoId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var logPhoto = await _context.LogPhotos.FindAsync(photoId);
        if (logPhoto == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        var filePath = Path.Combine(_env.WebRootPath, logPhoto.ImageURL.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        _context.LogPhotos.Remove(logPhoto);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }
}