using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface ITaskScheduleService
{
    Task<EmployeeWeeklyScheduleData> GetEmployeeWeeklyScheduleAsync(string staffId, DateTime? targetDate);
    Task<List<TaskSchedule>> GetMyTasksAsync(string staffId);
    Task<TaskSchedule?> GetByIdAsync(int taskId);

    Task<ServiceResult<TaskSchedule>> CreateAsync(TaskSchedule taskSchedule);
    Task<ServiceResult<TaskSchedule>> UpdateAsync(int taskId, TaskSchedule taskSchedule);
    Task<ServiceResult<bool>> DeleteAsync(int taskId);
}

public class EmployeeWeeklyScheduleData
{
    public DateTime StartOfWeek { get; set; }
    public List<TaskSchedule> Tasks { get; set; } = new();
}