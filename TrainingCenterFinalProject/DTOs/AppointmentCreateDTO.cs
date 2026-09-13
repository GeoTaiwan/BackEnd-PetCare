namespace PetCare.DTOs;

// 用來接收前端預約表單的 DTO
public class AppointmentCreateDTO
{
    public int PetID { get; set; }
    public string InstitutionID { get; set; } = null!;
    public string? ServiceType { get; set; }
    public string SelectedDate { get; set; } = null!; // 格式: YYYY-MM-DD
    public string SelectedTime { get; set; } = null!; // 格式: HH:mm
}