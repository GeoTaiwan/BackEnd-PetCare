
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Data;
using PetCare.Controllers;

public class AppointmentController : BaseController
{
    private readonly PetCareDbContext _context;

    public AppointmentController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: APPOINTMENTS
    public async Task<IActionResult> Index()    
    {
        //抓出目前使用者的InstitutionID
        string? instID = await _context.Employments
        .Where(e => e.UserID == LoginUserId)
        .Select(e => e.InstitutionID)
        .FirstOrDefaultAsync();

        // 2. 防呆檢查：如果該登入者不是員工（找不到 InstitutionID）
        if (string.IsNullOrEmpty(instID))
        {
            // 視你的需求決定，可以回傳錯誤訊息，或回傳空的預約清單
            return View(new List<Appointment>());
        }

        var app = await _context.Appointments
            .Where(a=>a.InstitutionID==instID)
            .ToListAsync();
        return View(app);
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

    // GET: APPOINTMENTS/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Create(string? instId)
    {
        // 一般使用者預約用
        // 如果沒有傳入機構ID (例如使用者是自己手打網址進來的)，可以導回列表頁
        if (string.IsNullOrEmpty(instId))
        {
            TempData["ErrorMessage"] = "請先選擇要預約的機構！";
            return RedirectToAction("IndexForReservation", "Institution");
        }

        // (選擇性 UX 提升) 查詢這家機構的名字，用 ViewBag 傳給畫面顯示，讓使用者安心
        var selectedInst = await _context.Institutions.FindAsync(instId);
        if (selectedInst != null)
        {
            ViewBag.InstitutionName = selectedInst.Name;
        }

        // 建立一個新的 Appointment 模型，並預先塞入使用者選好的機構 ID
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
