using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public class TaskScheduleManageService : ITaskScheduleManageService
{
    private readonly PetCareDbContext _context;

    public TaskScheduleManageService(PetCareDbContext context)
    {
        _context = context;
    }

    public async Task<TaskScheduleManageIndexData?> GetIndexDataAsync(string userId)
    {
        var institutionId = await _context.Employments
            .Where(e => e.UserID == userId)
            .Select(e => e.InstitutionID)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(institutionId)) return null;

        var institutionName = await _context.Institutions
            .Where(i => i.InstitutionID == institutionId)
            .Select(i => i.Name)
            .FirstOrDefaultAsync() ?? string.Empty;

        var taskSchedules = await _context.TaskSchedules
            .Include(t => t.Staff)
            .Include(t => t.Institution)
                .ThenInclude(i => i.Employments)
                    .ThenInclude(e => e.OccupationDetails)
            .Where(t => t.InstitutionID == institutionId)
            .ToListAsync();

        return new TaskScheduleManageIndexData
        {
            InstitutionId = institutionId,
            InstitutionName = institutionName,
            TaskSchedules = taskSchedules
        };
    }

    public async Task<TaskSchedule?> GetByIdAsync(int taskId)
    {
        return await _context.TaskSchedules
            .Include(t => t.Staff)
            .Include(t => t.Institution)
            .FirstOrDefaultAsync(t => t.TaskID == taskId);
    }

    public async Task<ServiceResult<TaskSchedule>> CreateAsync(TaskSchedule taskSchedule)
    {
        var result = new ServiceResult<TaskSchedule> { IsSuccess = false };

        _context.TaskSchedules.Add(taskSchedule);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = taskSchedule;
        return result;
    }

    public async Task<ServiceResult<TaskSchedule>> AssignEmployeeAsync(TaskSchedule taskSchedule)
    {
        var result = new ServiceResult<TaskSchedule> { IsSuccess = false };

        taskSchedule.CreatedDate = DateTime.Now;
        taskSchedule.EndTime = taskSchedule.StartTime.AddMinutes(30);
        taskSchedule.Status = "Active";

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