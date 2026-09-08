
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Data;
using PetCare.Controllers;

public class TaskScheduleController : BaseController
{
    private readonly PetCareDbContext _context;

    public TaskScheduleController(PetCareDbContext context)
    {
        _context = context;
    }

    // targetDate 參數讓使用者可以切換「上一週」、「下一週」
    public async Task<IActionResult> EmployeeSchedule(DateTime? targetDate)
    {
        // 1. 決定要顯示的基準日 (沒傳就用今天)
        DateTime baseDate = targetDate ?? DateTime.Today;

        // 2. 計算本週的「星期日」(StartOfWeek)
        // DayOfWeek.Sunday = 0, 所以 baseDate 減去它的 DayOfWeek 就能回到星期日
        int diff = baseDate.DayOfWeek - DayOfWeek.Sunday;
        DateTime startOfWeek = baseDate.AddDays(-1 * diff).Date;

        // 本週的結束邊界 (下週日 00:00:00，用來做範圍查詢)
        DateTime endOfWeek = startOfWeek.AddDays(7);

        // 3. 取得當前登入員工的 StaffID
        // 這裡假設你寫死 "EMP001" 測試，實際應用請替換成 User.Identity.Name 或 Session
        string currentStaffID = LoginUserId;

        // 4. 從資料庫撈取該員工「這一週」的所有任務
        var tasks = await _context.TaskSchedules
            .Include(t => t.Appointment) // 視需要關聯
            .ThenInclude(a=>a.Pet)
            .Include(a=>a.Staff)
            .Where(t => t.StaffID == currentStaffID
                     && t.StartTime >= startOfWeek
                     && t.StartTime < endOfWeek
                     && t.Status == "Active") // 如果你需要過濾狀態
            .ToListAsync();

        // 5. 將每週的起始日傳給 View，方便顯示標題與切換週次
        ViewBag.StartOfWeek = startOfWeek;

        return View(tasks);
    }

    // GET: TASKSCHEDULES
    public async Task<IActionResult> Index()    
    {
        var tasks = await _context.TaskSchedules
            .Where(t=>t.StaffID==LoginUserId)
            .ToListAsync();
        return View(tasks);
    }

    // GET: TASKSCHEDULES/Details/5
    public async Task<IActionResult> Details(int? taskid)
    {
        if (taskid == null)
        {
            return NotFound();
        }

        var taskschedule = await _context.TaskSchedules
            .FirstOrDefaultAsync(m => m.TaskID == taskid);
        if (taskschedule == null)
        {
            return NotFound();
        }

        return View(taskschedule);
    }

    // GET: TASKSCHEDULES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TASKSCHEDULES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TaskID,StaffID,InstitutionID,CreatedDate,StartTime,EndTime,TaskDescription,Status,AppointmentID,Staff,Institution,Appointment")] TaskSchedule taskschedule)
    {
        if (ModelState.IsValid)
        {
            _context.Add(taskschedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(taskschedule);
    }

    // GET: TASKSCHEDULES/Edit/5
    public async Task<IActionResult> Edit(int? taskid)
    {
        if (taskid == null)
        {
            return NotFound();
        }

        var taskschedule = await _context.TaskSchedules.FindAsync(taskid);
        if (taskschedule == null)
        {
            return NotFound();
        }
        return View(taskschedule);
    }

    // POST: TASKSCHEDULES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? taskid, [Bind("TaskID,StaffID,InstitutionID,CreatedDate,StartTime,EndTime,TaskDescription,Status,AppointmentID,Staff,Institution,Appointment")] TaskSchedule taskschedule)
    {
        if (taskid != taskschedule.TaskID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(taskschedule);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskScheduleExists(taskschedule.TaskID))
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
        return View(taskschedule);
    }

    // GET: TASKSCHEDULES/Delete/5
    public async Task<IActionResult> Delete(int? taskid)
    {
        if (taskid == null)
        {
            return NotFound();
        }

        var taskschedule = await _context.TaskSchedules
            .FirstOrDefaultAsync(m => m.TaskID == taskid);
        if (taskschedule == null)
        {
            return NotFound();
        }

        return View(taskschedule);
    }

    // POST: TASKSCHEDULES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? taskid)
    {
        var taskschedule = await _context.TaskSchedules.FindAsync(taskid);
        if (taskschedule != null)
        {
            _context.TaskSchedules.Remove(taskschedule);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TaskScheduleExists(int? taskid)
    {
        return _context.TaskSchedules.Any(e => e.TaskID == taskid);
    }
}
