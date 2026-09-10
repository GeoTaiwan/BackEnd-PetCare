using TrainingCenterFinalProject.Common;
using TrainingCenterFinalProject.Models;

namespace TrainingCenterFinalProject.Services;

public interface ITaskScheduleManageService
{
    Task<TaskScheduleManageIndexData?> GetIndexDataAsync(string userId);
    Task<TaskSchedule?> GetByIdAsync(int taskId);

    Task<ServiceResult<TaskSchedule>> CreateAsync(TaskSchedule taskSchedule);
    Task<ServiceResult<TaskSchedule>> AssignEmployeeAsync(TaskSchedule taskSchedule);
    Task<ServiceResult<TaskSchedule>> UpdateAsync(int taskId, TaskSchedule taskSchedule);

    Task<ServiceResult<bool>> DeleteAsync(int taskId);
}

public class TaskScheduleManageIndexData
{
    public string InstitutionId { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public List<TaskSchedule> TaskSchedules { get; set; } = new();
}