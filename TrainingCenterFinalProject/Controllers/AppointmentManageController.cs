using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

[Authorize(Roles = "VetManage,BeautyManage,NannyManage")]
public class AppointmentManageController : BaseController
{
    private readonly IAppointmentManageService _appointmentManageService;

    public AppointmentManageController(IAppointmentManageService appointmentManageService)
    {
        _appointmentManageService = appointmentManageService;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var instID = await _appointmentManageService.GetInstitutionIdByUserAsync(LoginUserId);
        if (string.IsNullOrEmpty(instID)) return NotFound();

        ViewBag.InstName = await _appointmentManageService.GetInstitutionNameAsync(instID);

        var app = await _appointmentManageService.GetInstitutionAppointmentsAsync(instID);
        var employees = await _appointmentManageService.GetInstitutionEmployeesAsync(instID);
        ViewBag.EmployeeList = new SelectList(employees, "UserID", "Name");

        return View(app);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(string? appointmentid)
    {
        if (string.IsNullOrEmpty(appointmentid)) return NotFound();

        var ok = await _appointmentManageService.AcceptAsync(appointmentid, LoginUserId);
        if (!ok) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string? appointmentid)
    {
        if (string.IsNullOrEmpty(appointmentid)) return NotFound();

        var ok = await _appointmentManageService.RejectAsync(appointmentid, LoginUserId);
        if (!ok) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string? appointmentid)
    {
        if (string.IsNullOrEmpty(appointmentid)) return NotFound();

        var appointment = await _appointmentManageService.GetByIdAsync(appointmentid);
        if (appointment == null) return NotFound();

        return View(appointment);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AppointmentID,PetID,InstitutionID,ServiceType,ApptDateTime,Status,Pet,Institution")] Appointment appointment)
    {
        if (!ModelState.IsValid) return View(appointment);

        var result = await _appointmentManageService.CreateAsync(appointment);
        if (!result.IsSuccess) return View(appointment);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? appointmentid)
    {
        if (string.IsNullOrEmpty(appointmentid)) return NotFound();

        var appointment = await _appointmentManageService.GetByIdAsync(appointmentid);
        if (appointment == null) return NotFound();

        return View(appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? appointmentid, [Bind("AppointmentID,PetID,InstitutionID,ServiceType,ApptDateTime,Status,Pet,Institution")] Appointment appointment)
    {
        if (string.IsNullOrEmpty(appointmentid)) return NotFound();
        if (!ModelState.IsValid) return View(appointment);

        var result = await _appointmentManageService.UpdateAsync(appointmentid, appointment);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string? appointmentid)
    {
        if (string.IsNullOrEmpty(appointmentid)) return NotFound();

        var appointment = await _appointmentManageService.GetByIdAsync(appointmentid);
        if (appointment == null) return NotFound();

        return View(appointment);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? appointmentid)
    {
        if (string.IsNullOrEmpty(appointmentid)) return NotFound();

        var ok = await _appointmentManageService.DeleteAsync(appointmentid);
        if (!ok) return NotFound();

        return RedirectToAction(nameof(Index));
    }
}