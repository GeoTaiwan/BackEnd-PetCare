using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface IAppointmentService
{
    Task<List<Appointment>> GetUserAppointmentsAsync(string userId, int petId = 0);
    Task<List<Pet>> GetUserPetsAsync(string userId);
    Task<List<Institution>> GetActiveInstitutionsAsync();

    Task<Appointment?> GetByIdAsync(string appointmentId);
    Task<string?> GetInstitutionNameAsync(string institutionId);

    List<string> GenerateAvailableTimeSlots(DateTime startTime, DateTime endTime, TimeSpan interval, DateTime selectedDate = default);

    Task<ServiceResult<Appointment>> CreateAsync(Appointment appointment, string selectedDate, string selectedTime);
    Task<ServiceResult<Appointment>> UpdateAsync(string id, Appointment appointment);
    Task<ServiceResult<int>> DeleteAsync(string id);
}