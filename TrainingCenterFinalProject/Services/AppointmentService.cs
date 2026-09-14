using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class AppointmentService : IAppointmentService
{
    private readonly PetCareDbContext _context;

    public AppointmentService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<Appointment>> GetUserAppointmentsAsync(string userId, int petId = 0)
    {
        var query = _context.Appointments
            .Include(a => a.Pet)
            .Include(a => a.Institution)
            .Where(a => a.Pet!.OwnerID == userId);

        if (petId != 0)
        {
            query = query.Where(a => a.PetID == petId);
        }

        return await query
            .OrderByDescending(a => a.ApptDateTime)
            .ToListAsync();
    }

    public async Task<List<Pet>> GetUserPetsAsync(string userId)
    {
        return await _context.Pets
            .Where(p => p.OwnerID == userId)
            .ToListAsync();
    }

    public async Task<List<Institution>> GetActiveInstitutionsAsync()
    {
        return await _context.Institutions
            .Where(i => i.StatusId == "S01")
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(string appointmentId)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
    }

    public async Task<string?> GetInstitutionNameAsync(string institutionId)
    {
        return await _context.Institutions
            .Where(i => i.InstitutionID == institutionId)
            .Select(i => i.Name)
            .FirstOrDefaultAsync();
    }

    public List<string> GenerateAvailableTimeSlots(DateTime startTime, DateTime endTime, TimeSpan interval, DateTime selectedDate = default)
    {
        var result = new List<string>();

        if (selectedDate != default)
        {
            startTime = selectedDate.Date.Add(startTime.TimeOfDay);
            endTime = selectedDate.Date.Add(endTime.TimeOfDay);
        }

        for (var time = startTime; time < endTime; time += interval)
        {
            result.Add(time.ToString("HH:mm"));
        }

        return result;
    }

    public async Task<ServiceResult<Appointment>> CreateAsync(Appointment appointment, string selectedDate, string selectedTime)
    {
        var result = new ServiceResult<Appointment> { IsSuccess = false };

        if (!DateTime.TryParse($"{selectedDate} {selectedTime}", out var apptDateTime))
        {
            result.ErrorMessage = "預約日期或時間格式錯誤。";
            return result;
        }

        appointment.AppointmentID = await GenerateAppointmentIdAsync(appointment.InstitutionID);
        appointment.Status = "已申請待確認";
        appointment.ApptDateTime = apptDateTime;
        appointment.ApplyTime = DateTime.Now;

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = appointment;
        return result;
    }

    public async Task<ServiceResult<Appointment>> UpdateAsync(string id, Appointment appointment)
    {
        var result = new ServiceResult<Appointment> { IsSuccess = false };

        if (id != appointment.AppointmentID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        try
        {
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = appointment;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Appointments.AnyAsync(a => a.AppointmentID == appointment.AppointmentID);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<ServiceResult<int>> DeleteAsync(string id)
    {
        var result = new ServiceResult<int> { IsSuccess = false };

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        var petId = appointment.PetID;
        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = petId;
        return result;
    }

    private async Task<string> GenerateAppointmentIdAsync(string institutionId)
    {
        var currentDateTime = DateTime.Now;
        var today = currentDateTime.Date;
        var tomorrow = today.AddDays(1);
        var datePart = today.ToString("yyyyMMdd");
        var count = await _context.Appointments
            .CountAsync(a => a.InstitutionID == institutionId &&
            a.ApplyTime >= today &&
            a.ApplyTime < tomorrow);

        var serialNumber = count + 1;

        return $"A{datePart}-{institutionId}-{serialNumber:D3}";
    }
}