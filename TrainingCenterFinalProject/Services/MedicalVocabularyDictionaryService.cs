using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class MedicalVocabularyDictionaryService : IMedicalVocabularyDictionaryService
{
    private readonly PetCareDbContext _context;

    public MedicalVocabularyDictionaryService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<MedicalVocabularyDictionary>> GetAllAsync()
    {
        return await _context.MedicalVocabularyDictionaries
            .OrderBy(x => x.TermCode)
            .ToListAsync();
    }

    public async Task<MedicalVocabularyDictionary?> GetByIdAsync(int termId)
    {
        return await _context.MedicalVocabularyDictionaries
            .FirstOrDefaultAsync(m => m.TermID == termId);
    }

    public async Task<ServiceResult<MedicalVocabularyDictionary>> CreateAsync(MedicalVocabularyDictionary model)
    {
        var result = new ServiceResult<MedicalVocabularyDictionary> { IsSuccess = false };

        model.UpdatedAt = DateTime.Now;
        _context.MedicalVocabularyDictionaries.Add(model);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = model;
        return result;
    }

    public async Task<ServiceResult<MedicalVocabularyDictionary>> UpdateAsync(int termId, MedicalVocabularyDictionary model)
    {
        var result = new ServiceResult<MedicalVocabularyDictionary> { IsSuccess = false };

        if (termId != model.TermID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        model.UpdatedAt = DateTime.Now;

        try
        {
            _context.Update(model);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = model;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.MedicalVocabularyDictionaries.AnyAsync(e => e.TermID == model.TermID);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int termId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var entity = await _context.MedicalVocabularyDictionaries.FindAsync(termId);
        if (entity == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        _context.MedicalVocabularyDictionaries.Remove(entity);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }
}