using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

[Authorize(Roles = "VetManage,BeautyManage,NannyManage")]
public class TaskScheduleManageController : BaseController
{
    private readonly ITaskScheduleManageService _taskScheduleManageService;

    public TaskScheduleManageController(ITaskScheduleManageService taskScheduleManageService)
    {
        _taskScheduleManageService = taskScheduleManageService;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var data = await _taskScheduleManageService.GetIndexDataAsync(LoginUserId);
        if (data == null) return BadRequest();

        ViewBag.InstName = data.InstitutionName;
        return View(data.TaskSchedules);
    }

    public async Task<IActionResult> Details(int? taskid)
    {
        if (taskid == null) return NotFound();

        var task = await _taskScheduleManageService.GetByIdAsync(taskid.Value);
        if (task == null) return NotFound();

        return View(task);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TaskID,StaffID,InstitutionID,CreatedDate,StartTime,EndTime,TaskDescription,Status,Staff,Institution")] TaskSchedule taskschedule)
    {
        if (!ModelState.IsValid) return View(taskschedule);

        var result = await _taskScheduleManageService.CreateAsync(taskschedule);
        if (!result.IsSuccess) return View(taskschedule);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignEmployee(TaskSchedule taskschedule)
    {
        if (taskschedule == null) return NotFound();

        ModelState.Remove("CreatedDate");
        ModelState.Remove("EndTime");
        ModelState.Remove("Status");

        if (!ModelState.IsValid)
        {
            TempData[$"ErrMsg_{taskschedule.AppointmentID}"] = "驗證不通過，請檢查輸入的資料是否正確。";
            return RedirectToAction(nameof(Index), "AppointmentManage");
        }

        var result = await _taskScheduleManageService.AssignEmployeeAsync(taskschedule);
        if (!result.IsSuccess)
        {
            TempData[$"ErrMsg_{taskschedule.AppointmentID}"] = "建立任務失敗。";
        }

        return RedirectToAction(nameof(Index), "AppointmentManage");
    }

    public async Task<IActionResult> Edit(int? taskid)
    {
        if (taskid == null) return NotFound();

        var task = await _taskScheduleManageService.GetByIdAsync(taskid.Value);
        if (task == null) return NotFound();

        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? taskid, TaskSchedule taskschedule)
    {
        if (taskid == null) return NotFound();
        if (!ModelState.IsValid) return View(taskschedule);

        var result = await _taskScheduleManageService.UpdateAsync(taskid.Value, taskschedule);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? taskid)
    {
        if (taskid == null) return NotFound();

        var result = await _taskScheduleManageService.DeleteAsync(taskid.Value);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }
}