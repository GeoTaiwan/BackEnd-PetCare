using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IMedicalVocabularyDictionaryService
{
    Task<List<MedicalVocabularyDictionary>> GetAllAsync();
    Task<MedicalVocabularyDictionary?> GetByIdAsync(int termId);

    Task<ServiceResult<MedicalVocabularyDictionary>> CreateAsync(MedicalVocabularyDictionary model);
    Task<ServiceResult<MedicalVocabularyDictionary>> UpdateAsync(int termId, MedicalVocabularyDictionary model);
    Task<ServiceResult<bool>> DeleteAsync(int termId);
}