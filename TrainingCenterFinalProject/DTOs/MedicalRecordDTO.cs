namespace PetCare.DTOs;

public class MedicalRecordDTO
{
    public int RecordID { get; set; }
    public DateTime VisitDate { get; set; }
    public string VetID { get; set; } = null!;
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? TestResultsFile { get; set; }

    // 如果你需要顯示獸醫的真實姓名，且 UserProfile 裡有 Name 的話，
    // 可以新增一個 public string? VetName { get; set; }，並在稍後轉換時對應。
    public string? VetName { get; set; }
}
