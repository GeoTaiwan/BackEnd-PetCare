using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IOccupationsService
{
    Task<List<Occupations>> GetAllAsync();
    Task<Occupations?> GetByIdAsync(string occupationId);

    Task<ServiceResult<Occupations>> CreateAsync(Occupations occupations);
    Task<ServiceResult<Occupations>> UpdateAsync(string occupationId, Occupations occupations);
    Task<ServiceResult<bool>> DeleteAsync(string occupationId);
}