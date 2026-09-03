using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class AppointmentController : BaseController
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<IActionResult> Index(int petid = 0)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        await PopulatePetListAsync(petid);
        var appointments = await _appointmentService.GetUserAppointmentsAsync(LoginUserId, petid);
        return View(appointments);
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();

        return View(appointment);
    }

    [HttpGet]
    public async Task<IActionResult> Create(string? instId)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        if (string.IsNullOrEmpty(instId))
        {
            TempData["ErrorMessage"] = "請先選擇要預約的機構！";
            return RedirectToAction("IndexForReservation", "Institution");
        }

        await PopulateCreateViewDataAsync(0, instId);

        var selectedInstName = await _appointmentService.GetInstitutionNameAsync(instId);
        if (!string.IsNullOrEmpty(selectedInstName))
        {
            ViewBag.InstitutionName = selectedInstName;
        }

        return View(new Appointment { InstitutionID = instId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment appointment, string selectedDate, string selectedTime)
    {
        ModelState.Remove("AppointmentID");
        ModelState.Remove("Status");
        ModelState.Remove("ApptDateTime");
        ModelState.Remove("ApplyTime");

        if (!ModelState.IsValid)
        {
            await PopulateCreateViewDataAsync(appointment.PetID, appointment.InstitutionID);
            return View(appointment);
        }

        var result = await _appointmentService.CreateAsync(appointment, selectedDate, selectedTime);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            await PopulateCreateViewDataAsync(appointment.PetID, appointment.InstitutionID);
            return View(appointment);
        }

        return RedirectToAction(nameof(Details), "Pet", new { petid = appointment.PetID });
    }

    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();

        return View(appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? id, [Bind("AppointmentID,PetID,InstitutionID,ServiceType,ApptDateTime,Status,Pet,Institution")] Appointment appointment)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        if (!ModelState.IsValid) return View(appointment);

        var result = await _appointmentService.UpdateAsync(id, appointment);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND")
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();

        return View(appointment);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var result = await _appointmentService.DeleteAsync(id);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND")
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), new { petid = result.Data });
    }

    private async Task PopulatePetListAsync(int selectedPetId = 0)
    {
        var pets = await _appointmentService.GetUserPetsAsync(LoginUserId!);
        ViewBag.PetList = new SelectList(pets, "PetID", "Name", selectedPetId == 0 ? null : selectedPetId);
    }

    private async Task PopulateCreateViewDataAsync(int selectedPetId = 0, string? selectedInstitutionId = null)
    {
        await PopulatePetListAsync(selectedPetId);

        var institutions = await _appointmentService.GetActiveInstitutionsAsync();
        ViewBag.InstitutionList = new SelectList(institutions, "InstitutionID", "Name", selectedInstitutionId);

        var slots = _appointmentService.GenerateAvailableTimeSlots(
            DateTime.Today.AddHours(9),
            DateTime.Today.AddHours(18),
            TimeSpan.FromMinutes(30));

        ViewBag.TimeSlots = slots.Select(s => new SelectListItem { Value = s, Text = s }).ToList();
    }
}
