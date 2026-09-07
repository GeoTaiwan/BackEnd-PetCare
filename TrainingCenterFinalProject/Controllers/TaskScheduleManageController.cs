
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Data;
using PetCare.Controllers;
using Microsoft.AspNetCore.Authorization;
[Authorize(Roles = "VetManage,BeautyManage,NannyManage")]
public class TaskScheduleManageController : BaseController
{
    private readonly PetCareDbContext _context;

    public TaskScheduleManageController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: TASKSCHEDULES
    public async Task<IActionResult> Index()    
    {
        var taskSchedules = await _context.TaskSchedules
            .Include(t=>t.Staff)
            .Include(t=>t.Institution)
            .ThenInclude(i=>i.Employments).ThenInclude(e=>e.OccupationDetails)
            .ToListAsync();
        var currentUserInstitution = _context.Employments
            .Where(e => e.UserID == LoginUserId)
            .Select(e => e.InstitutionID)
            .FirstOrDefault();
        ViewBag.InstName = _context.Institutions
            .Where(i=>i.InstitutionID == currentUserInstitution)
            .Select(i=>i.Name)
            .FirstOrDefault();
        return View(taskSchedules);
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
    public async Task<IActionResult> Create([Bind("TaskID,StaffID,InstitutionID,CreatedDate,StartTime,EndTime,TaskDescription,Status,Staff,Institution")] TaskSchedule taskschedule)
    {
        if (ModelState.IsValid)
        {
            _context.Add(taskschedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(taskschedule);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignEmployee(TaskSchedule taskschedule)
    {
        if (taskschedule == null)
        {
            return NotFound();
        }
        ModelState.Remove("CreatedDate");
        ModelState.Remove("StartTime");
        ModelState.Remove("EndTime");
        ModelState.Remove("Status");
        taskschedule.CreatedDate = DateTime.Now;
        taskschedule.EndTime =taskschedule.StartTime.AddMinutes(30); // Default duration of 30 minutes
        taskschedule.Status = "Active"; // Default status
        if (ModelState.IsValid)
        {
            _context.Add(taskschedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),"AppointmentManage");
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
    public async Task<IActionResult> Edit(int? taskid, [Bind("TaskID,StaffID,InstitutionID,CreatedDate,StartTime,EndTime,TaskDescription,Status,Staff,Institution")] TaskSchedule taskschedule)
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

    //// GET: TASKSCHEDULES/Delete/5
    //public async Task<IActionResult> Delete(int? taskid)
    //{
    //    if (taskid == null)
    //    {
    //        return NotFound();
    //    }

    //    var taskschedule = await _context.TaskSchedules
    //        .FirstOrDefaultAsync(m => m.TaskID == taskid);
    //    if (taskschedule == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(taskschedule);
    //}

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
