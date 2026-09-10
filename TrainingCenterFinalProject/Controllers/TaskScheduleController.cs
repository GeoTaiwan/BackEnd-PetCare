using Microsoft.AspNetCore.Mvc;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class TaskScheduleController : BaseController
{
    private readonly ITaskScheduleService _taskScheduleService;

    public TaskScheduleController(ITaskScheduleService taskScheduleService)
    {
        _taskScheduleService = taskScheduleService;
    }

    // targetDate 參數讓使用者切換週次
    public async Task<IActionResult> EmployeeSchedule(DateTime? targetDate)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var data = await _taskScheduleService.GetEmployeeWeeklyScheduleAsync(LoginUserId, targetDate);
        ViewBag.StartOfWeek = data.StartOfWeek;
        return View(data.Tasks);
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var tasks = await _taskScheduleService.GetMyTasksAsync(LoginUserId);
        return View(tasks);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var taskSchedule = await _taskScheduleService.GetByIdAsync(id.Value);
        if (taskSchedule == null) return NotFound();

        return View(taskSchedule);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TaskID,StaffID,InstitutionID,CreatedDate,StartTime,EndTime,TaskDescription,Status,AppointmentID,Staff,Institution,Appointment")] TaskSchedule taskSchedule)
    {
        if (!ModelState.IsValid) return View(taskSchedule);

        var result = await _taskScheduleService.CreateAsync(taskSchedule);
        if (!result.IsSuccess) return View(taskSchedule);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var taskSchedule = await _taskScheduleService.GetByIdAsync(id.Value);
        if (taskSchedule == null) return NotFound();

        return View(taskSchedule);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("TaskID,StaffID,InstitutionID,CreatedDate,StartTime,EndTime,TaskDescription,Status,AppointmentID,Staff,Institution,Appointment")] TaskSchedule taskSchedule)
    {
        if (id == null) return NotFound();
        if (!ModelState.IsValid) return View(taskSchedule);

        var result = await _taskScheduleService.UpdateAsync(id.Value, taskSchedule);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var taskSchedule = await _taskScheduleService.GetByIdAsync(id.Value);
        if (taskSchedule == null) return NotFound();

        return View(taskSchedule);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null) return NotFound();

        var result = await _taskScheduleService.DeleteAsync(id.Value);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }
}