
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
        await getPetList();
        var institutions = await _context.Institutions
            .Where(i => i.StatusId == "S01")
            .ToListAsync();
        ViewBag.InstitutionList = new SelectList(institutions, "InstitutionID", "Name");
    }

    private async Task getPetList(int petid = 0)
    {
        var pets = await _context.Pets
            .Where(p => p.OwnerID == LoginUserId)
            .ToListAsync();
        ViewBag.PetList = new SelectList(pets, "PetID", "Name",petid>0?petid:0);
    }

    // GET: APPOINTMENTS
    public async Task<IActionResult> Index(int petid=0)    
    {
        //機構用的Appointment已經在AppointmentManageController裡面做了，所以這裡只要抓出使用者的寵物預約即可
        await getPetList(petid);
        var appointments = await _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Institution)
                .Where(a => a.Pet.OwnerID == LoginUserId)
                .OrderByDescending(a => a.ApptDateTime).ToListAsync();
        if (petid != 0)
        {
            appointments = appointments.Where(a=>a.PetID == petid).ToList();
            return View(appointments);
        }
        //petid = await _context.Pets
        //    .Where(p => p.OwnerID == LoginUserId)
        //    .Select(p => p.PetID)
        //    .FirstOrDefaultAsync();
        //appointments = appointments.Where(a => a.PetID == petid).ToList();
        
        return View(appointments);
        
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

    // 產生AppointmentID的方法，長度為19個字元，ex：A20260803-I00001-001，A開頭，後面接日期、機構ID、流水號
    private string GenerateAppointmentID(string institutionID)
    {
        var datePart = DateTime.Now.ToString("yyyyMMdd");
        var serialNumber = _context.Appointments.Count(a => a.InstitutionID == institutionID && a.ApptDateTime.Date == DateTime.Today) + 1;
        return $"A{datePart}-{institutionID}-{serialNumber:D3}";
    }

    // POST: APPOINTMENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment appointment)
    {
        ModelState.Remove("AppointmentID"); // 移除 AppointmentID 的驗證，因為我們會自動生成它
        ModelState.Remove("Status"); // 移除 Status 的驗證，因為我們會自動生成它
        appointment.AppointmentID = GenerateAppointmentID(appointment.InstitutionID);
        appointment.Status = "已申請待確認"; // 預設狀態為 Pending
        if (ModelState.IsValid)
        {
            _context.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details),"Pet", new { petid = appointment.PetID });
        }
        await getList(); // 如果驗證失敗，重新取得下拉選單資料
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
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        var petid = appointment.PetID.ToString();
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index),new {petid=petid});
    }

    private bool AppointmentExists(string? id)
    {
        return _context.Appointments.Any(e => e.AppointmentID == id);
    }
}
