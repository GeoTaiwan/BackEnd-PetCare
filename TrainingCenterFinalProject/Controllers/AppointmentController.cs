
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

    //一個產生可預約的時間清單的方法，回傳List<SelectListItem>，用於下拉選單
    //機構可設定營業時間、間隔時間，這裡假設營業時間從早上9點到下午6點，每30分鐘一個時段
    //使用者可選日期後，會產生該日期的可預約時間清單
    private List<SelectListItem> GenerateAvailableTimeSlots(DateTime startTime, DateTime endTime, TimeSpan interval, DateTime selectedDate = default)
    {
        var timeSlots = new List<SelectListItem>();
        if (selectedDate != default) {
            startTime = selectedDate.Date.Add(startTime.TimeOfDay);
            endTime = selectedDate.Date.Add(endTime.TimeOfDay);
        }
        for (var time = startTime; time < endTime; time += interval)
        {
            timeSlots.Add(new SelectListItem
            {
                Value = time.ToString("HH:mm"),
                Text = time.ToString("HH:mm")
            });
        }

        return timeSlots;
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
        ViewBag.TimeSlots = GenerateAvailableTimeSlots(DateTime.Today.AddHours(9), DateTime.Today.AddHours(18), TimeSpan.FromMinutes(30));
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
    private async Task<string> GenerateAppointmentID(string institutionID)
    {
        var datePart = DateTime.Now.ToString("yyyyMMdd");
        var serialNumber = await _context.Appointments.CountAsync(a => a.InstitutionID == institutionID && a.ApplyTime.Date == DateTime.Today) + 1;
        return $"A{datePart}-{institutionID}-{serialNumber:D3}";
    }

    // POST: APPOINTMENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment appointment, string selectedDate, string selectedTime)
    {
        ModelState.Remove("AppointmentID"); // 移除 AppointmentID 的驗證，因為我們會自動生成它
        ModelState.Remove("Status"); // 移除 Status 的驗證，因為我們會自動生成它
        ModelState.Remove("ApptDateTime"); // 移除 ApptDateTime 的驗證，因為我們會自動生成它
        appointment.AppointmentID = await GenerateAppointmentID(appointment.InstitutionID);
        appointment.Status = "已申請待確認"; // 預設狀態為 Pending
        appointment.ApptDateTime = DateTime.Parse($"{selectedDate} {selectedTime}");
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
