namespace PetCare.DTOs;

public class MedicalRecordCreateForm
{
    public DateTime VisitDate { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public IFormFile? UploadFile { get; set; }
}