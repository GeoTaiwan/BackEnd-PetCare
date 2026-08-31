
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Data;
using PetCare.Controllers;
using Microsoft.AspNetCore.Authorization;
[Authorize(Roles = "Vet,Beauty,Nanny,VetManage,BeautyManage,NannyManage")]
public class AppointmentManageController : BaseController
{
    private readonly PetCareDbContext _context;

    public AppointmentManageController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: APPOINTMENTS
    public async Task<IActionResult> Index()    
    {
        // 1. 取得目前登入者的 InstitutionID
        string? instID = await _context.Employments
            .Where(e => e.UserID == LoginUserId)
            .Select(e => e.InstitutionID)
            .FirstOrDefaultAsync();

        var app = await _context.Appointments
            .Include(a=>a.Pet)
            .Include(a=>a.Institution)
            .Where(a => a.InstitutionID == instID)
            .OrderByDescending(a => a.ApptDateTime)
            .ToListAsync();
        return View(app);
    }

    // POST: APPOINTMENTS/Accept/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(string? appointmentid)
    {
        if (appointmentid == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments.FindAsync(appointmentid);
        if (appointment == null)
        {
            return NotFound();
        }

        appointment.Status = "預約成功";
        _context.Update(appointment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: APPOINTMENTS/Reject/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string? appointmentid)
    {
        if (appointmentid == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments.FindAsync(appointmentid);
        if (appointment == null)
        {
            return NotFound();
        }

        appointment.Status = "預約失敗";
        _context.Update(appointment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: APPOINTMENTS/Details/5
    public async Task<IActionResult> Details(string? appointmentid)
    {
        if (appointmentid == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(m => m.AppointmentID == appointmentid);
        if (appointment == null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    // GET: APPOINTMENTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: APPOINTMENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AppointmentID,PetID,InstitutionID,ServiceType,ApptDateTime,Status,Pet,Institution")] Appointment appointment)
    {
        if (ModelState.IsValid)
        {
            _context.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(appointment);
    }

    // GET: APPOINTMENTS/Edit/5
    public async Task<IActionResult> Edit(string? appointmentid)
    {
        if (appointmentid == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments.FindAsync(appointmentid);
        if (appointment == null)
        {
            return NotFound();
        }
        return View(appointment);
    }

    // POST: APPOINTMENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? appointmentid, [Bind("AppointmentID,PetID,InstitutionID,ServiceType,ApptDateTime,Status,Pet,Institution")] Appointment appointment)
    {
        if (appointmentid != appointment.AppointmentID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(appointment.AppointmentID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(appointment);
    }

    // GET: APPOINTMENTS/Delete/5
    public async Task<IActionResult> Delete(string? appointmentid)
    {
        if (appointmentid == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(m => m.AppointmentID == appointmentid);
        if (appointment == null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    // POST: APPOINTMENTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? appointmentid)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentid);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AppointmentExists(string? appointmentid)
    {
        return _context.Appointments.Any(e => e.AppointmentID == appointmentid);
    }
}
