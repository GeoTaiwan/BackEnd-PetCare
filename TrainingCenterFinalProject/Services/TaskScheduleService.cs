using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class TaskScheduleService : ITaskScheduleService
{
    private readonly PetCareDbContext _context;

    public TaskScheduleService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeWeeklyScheduleData> GetEmployeeWeeklyScheduleAsync(string staffId, DateTime? targetDate)
    {
        DateTime baseDate = targetDate ?? DateTime.Today;
        int diff = baseDate.DayOfWeek - DayOfWeek.Sunday;
        DateTime startOfWeek = baseDate.AddDays(-1 * diff).Date;
        DateTime endOfWeek = startOfWeek.AddDays(7);

        var tasks = await _context.TaskSchedules
            .Include(t => t.Appointment).ThenInclude(a => a.Pet)
            .Include(t => t.Staff)
            .Where(t => t.StaffID == staffId
                     && t.StartTime >= startOfWeek
                     && t.StartTime < endOfWeek
                     && t.Status == "Active")
            .ToListAsync();

        return new EmployeeWeeklyScheduleData
        {
            StartOfWeek = startOfWeek,
            Tasks = tasks
        };
    }

    public async Task<List<TaskSchedule>> GetMyTasksAsync(string staffId)
    {
        return await _context.TaskSchedules
            .Where(t => t.StaffID == staffId)
            .OrderByDescending(t => t.StartTime)
            .ToListAsync();
    }

    public async Task<TaskSchedule?> GetByIdAsync(int taskId)
    {
        return await _context.TaskSchedules
            .FirstOrDefaultAsync(m => m.TaskID == taskId);
    }

    public async Task<ServiceResult<TaskSchedule>> CreateAsync(TaskSchedule taskSchedule)
    {
        var result = new ServiceResult<TaskSchedule> { IsSuccess = false };

        if (taskSchedule.CreatedDate == default)
        {
            taskSchedule.CreatedDate = DateTime.Now;
        }

        _context.TaskSchedules.Add(taskSchedule);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = taskSchedule;
        return result;
    }

    public async Task<ServiceResult<TaskSchedule>> UpdateAsync(int taskId, TaskSchedule taskSchedule)
    {
        var result = new ServiceResult<TaskSchedule> { IsSuccess = false };

        if (taskId != taskSchedule.TaskID)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        try
        {
            _context.Update(taskSchedule);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = taskSchedule;
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.TaskSchedules.AnyAsync(e => e.TaskID == taskSchedule.TaskID);
            if (!exists)
            {
                result.ErrorMessage = "NOT_FOUND";
                return result;
            }

            throw;
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int taskId)
    {
        var result = new ServiceResult<bool> { IsSuccess = false };

        var taskSchedule = await _context.TaskSchedules.FindAsync(taskId);
        if (taskSchedule == null)
        {
            result.ErrorMessage = "NOT_FOUND";
            return result;
        }

        _context.TaskSchedules.Remove(taskSchedule);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = true;
        return result;
    }
}