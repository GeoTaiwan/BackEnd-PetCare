
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.Controllers;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

public class AppointmentController : BaseController
{
    private readonly PetCareDbContext _context;

    public AppointmentController(PetCareDbContext context)
    {
        _context = context;
    }

    private async Task getList()
    {
        //取得使用者寵物清單
        var pets = await _context.Pets
            .Where(p => p.OwnerID == LoginUserId)
            .ToListAsync();
        ViewBag.PetList = new SelectList(pets, "PetID", "Name");
        var institutions = await _context.Institutions
            .Where(i => i.StatusId == "S01")
            .ToListAsync();
        ViewBag.InstitutionList = new SelectList(institutions, "InstitutionID", "Name");
    }

    // GET: APPOINTMENTS
    public async Task<IActionResult> Index(int petid=0)    
    {
        if (petid == 0) { 
        return NotFound();
        }
        if (petid != 0)
        {
            var appointments = await _context.Appointments
                .Where(a => a.PetID == petid)
                .ToListAsync();
            
            //抓出目前使用者的InstitutionID
            string? instID = await _context.Employments
            .Where(e => e.UserID == LoginUserId)
            .Select(e => e.InstitutionID)
            .FirstOrDefaultAsync();

            // 2. 防呆檢查：如果該登入者不是員工（找不到 InstitutionID）
            if (string.IsNullOrEmpty(instID))
            {
                // 視你的需求決定，可以回傳錯誤訊息，或回傳空的預約清單
                return View(appointments);
            }
            var app = await _context.Appointments
            .Where(a => a.InstitutionID == instID)
            .ToListAsync();
            return View(app);
        }
        
        return BadRequest();
        
    }

    // GET: APPOINTMENTS/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(m => m.AppointmentID == id);
        if (appointment == null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    // 合併後的 GET: APPOINTMENTS/Create
    [HttpGet]
    public async Task<IActionResult> Create(string? instId)
    {
        // 一般使用者預約用
        if (string.IsNullOrEmpty(instId))
        {
            TempData["ErrorMessage"] = "請先選擇要預約的機構！";
            return RedirectToAction("IndexForReservation", "Institution");
        }

        // 🌟 必須呼叫這個，畫面才會有 PetList 和 InstitutionList！
        await getList();

        var selectedInst = await _context.Institutions.FindAsync(instId);
        if (selectedInst != null)
        {
            ViewBag.InstitutionName = selectedInst.Name;
        }

        var model = new Appointment
        {
            InstitutionID = instId
        };

        return View(model);
    }

    // POST: APPOINTMENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment appointment)
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
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments.FindAsync(id);
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
    public async Task<IActionResult> Edit(string? id, [Bind("AppointmentID,PetID,InstitutionID,ServiceType,ApptDateTime,Status,Pet,Institution")] Appointment appointment)
    {
        if (id != appointment.AppointmentID)
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
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(m => m.AppointmentID == id);
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

    private bool AppointmentExists(string? id)
    {
        return _context.Appointments.Any(e => e.AppointmentID == id);
    }
}
