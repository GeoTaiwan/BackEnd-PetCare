using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IAppointmentManageService
{
    Task<string?> GetInstitutionIdByUserAsync(string userId);
    Task<string?> GetInstitutionNameAsync(string institutionId);
    Task<List<Appointment>> GetInstitutionAppointmentsAsync(string institutionId);
    Task<List<EmployeeOption>> GetInstitutionEmployeesAsync(string institutionId);

    Task<bool> AcceptAsync(string appointmentId, string? handlerUserId);
    Task<bool> RejectAsync(string appointmentId, string? handlerUserId);

    Task<Appointment?> GetByIdAsync(string appointmentId);
    Task<ServiceResult<Appointment>> CreateAsync(Appointment appointment);
    Task<ServiceResult<Appointment>> UpdateAsync(string appointmentId, Appointment appointment);
    Task<bool> DeleteAsync(string appointmentId);
}

public class EmployeeOption
{
    public string UserID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}