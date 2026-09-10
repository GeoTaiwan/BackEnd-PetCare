using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class AppointmentManageService : IAppointmentManageService
{
    private readonly PetCareDbContext _context;

    public AppointmentManageService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetInstitutionIdByUserAsync(string userId)
    {
        return await _context.Employments
            .Where(e => e.UserID == userId)
            .Select(e => e.InstitutionID)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetInstitutionNameAsync(string institutionId)
    {
        return await _context.Institutions
            .Where(i => i.InstitutionID == institutionId)
            .Select(i => i.Name)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Appointment>> GetInstitutionAppointmentsAsync(string institutionId)
    {
        return await _context.Appointments
            .Include(a => a.Pet)
            .Include(a => a.Institution)
            .ThenInclude(i => i.AuthorizationRecords.Where(ar =>
                ar.GrantedDate < DateTime.Now &&
                ar.RevokedDate >= DateTime.Now))
            .Include(a => a.Employee)
            .Include(a => a.TaskSchedule)
            .ThenInclude(ts => ts.Staff)
            .Where(a => a.InstitutionID == institutionId)
            .OrderByDescending(a => a.ApplyTime)
            .ToListAsync();
    }

    public async Task<List<EmployeeOption>> GetInstitutionEmployeesAsync(string institutionId)
    {
        return await _context.Employments
            .Where(e => e.InstitutionID == institutionId)
            .Select(e => new EmployeeOption
            {
                UserID = e.UserID,
                Name = e.UserProfile!.Name
            })
            .ToListAsync();
    }

    public async Task<bool> AcceptAsync(string appointmentId, string? handlerUserId)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null) return false;

        appointment.HandleTime = DateTime.Now;
        appointment.EmployeeID = handlerUserId;
        appointment.Status = "預約成功";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAsync(string appointmentId, string? handlerUserId)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null) return false;

        appointment.HandleTime = DateTime.Now;
        appointment.EmployeeID = handlerUserId;
        appointment.Status = "預約失敗";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Appointment?> GetByIdAsync(string appointmentId)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
    }

    public async Task<ServiceResult<Appointment>> CreateAsync(Appointment appointment)
    {
        var result = new ServiceResult<Appointment> { IsSuccess = false };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = appointment;
        return result;
    }

    public async Task<ServiceResult<Appointment>> UpdateAsync(string appointmentId, Appointment appointment)
    {
        var result = new ServiceResult<Appointment> { IsSuccess = false };

        if (appointmentId != appointment.AppointmentID)
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

    public async Task<bool> DeleteAsync(string appointmentId)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null) return false;

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }
}