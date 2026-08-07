
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Security.Claims;
using TrainingCenterFinalProject.Models;
//飼主用controller
//獸醫端另算

public class MedicalRecordController : Controller
{
    private readonly PetCareDbContext _context;
    private readonly IWebHostEnvironment _env;

    public MedicalRecordController(PetCareDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: MEDICALRECORDS
    public async Task<IActionResult> Index(int petid=0)    
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var myRecords = _context.MedicalRecords
            .Include(m => m.Pet)
            .Include(m => m.Vet)
            .Where(m => m.Pet.OwnerID == currentUserId)
            .AsQueryable();

        if (petid > 0)
        {
            myRecords = myRecords.Where(r=>r.PetID == petid);
        }

        var pet = await _context.Pets.Where(p=>p.OwnerID==currentUserId).ToListAsync();
        ViewBag.noPet = !pet.Any();
        
        // 建立 SelectList，參數依序為：資料來源、Value 欄位、Text 欄位、當前選中的值
        ViewBag.PetList = new SelectList(pet,"PetID","Name",petid);
        ViewData["petid"] = petid;

        return View(await myRecords.ToListAsync());
    }

    // GET: MEDICALRECORDS/Details/5
    public async Task<IActionResult> Details(int? recordid)
    {
        if (recordid == null)
        {
            return NotFound();
        }

        var medicalrecord = await _context.MedicalRecords
            .FirstOrDefaultAsync(m => m.RecordID == recordid);
        if (medicalrecord == null)
        {
            return NotFound();
        }

        return View(medicalrecord);
    }

    // GET: MEDICALRECORDS/Create
    public async Task<IActionResult> Create(int petid=0)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var pet = await _context.Pets.Where(p => p.OwnerID == currentUserId).ToListAsync();
        if (pet != null) { 
            if (petid == 0)
        {
            petid=pet.FirstOrDefault().PetID;
        }
        //var pet = await _context.Pets.Where(p => p.OwnerID == currentUserId).ToListAsync();
        ViewBag.PetList = new SelectList(pet, "PetID", "Name",petid);

        return View();
        }
        return NotFound();
    }

    // POST: MEDICALRECORDS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RecordID,PetID,VetID,VisitDate,Symptoms,TestResultsFile,Diagnosis,Pet,Vet,RecordDiagnoses,Prescriptions")] MedicalRecord medicalrecord)
    {
        var pet = await _context.Pets.ToListAsync();
        if (ModelState.IsValid)
        {
            _context.Add(medicalrecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.PetList = new SelectList(pet, "PetID", "Name", medicalrecord.PetID);
        return View(medicalrecord);
    }

    // GET: MEDICALRECORDS/Edit/5
    public async Task<IActionResult> Edit(int? recordid)
    {
        if (recordid == null)
        {
            return NotFound();
        }

        var medicalrecord = await _context.MedicalRecords.FindAsync(recordid);
        if (medicalrecord == null)
        {
            return NotFound();
        }
        return View(medicalrecord);
    }

    // POST: MEDICALRECORDS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? recordid, [Bind("RecordID,PetID,VetID,VisitDate,Symptoms,TestResultsFile,Diagnosis,Pet,Vet,RecordDiagnoses,Prescriptions")] MedicalRecord medicalrecord)
    {
        if (recordid != medicalrecord.RecordID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(medicalrecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalRecordExists(medicalrecord.RecordID))
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
        return View(medicalrecord);
    }

    // GET: MEDICALRECORDS/Delete/5
    public async Task<IActionResult> Delete(int? recordid)
    {
        if (recordid == null)
        {
            return NotFound();
        }

        var medicalrecord = await _context.MedicalRecords
            .FirstOrDefaultAsync(m => m.RecordID == recordid);
        if (medicalrecord == null)
        {
            return NotFound();
        }

        return View(medicalrecord);
    }

    // POST: MEDICALRECORDS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? recordid)
    {
        var medicalrecord = await _context.MedicalRecords.FindAsync(recordid);
        if (medicalrecord != null)
        {
            _context.MedicalRecords.Remove(medicalrecord);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MedicalRecordExists(int? recordid)
    {
        return _context.MedicalRecords.Any(e => e.RecordID == recordid);
    }
}
